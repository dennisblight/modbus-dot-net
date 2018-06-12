using System;
using System.Linq;
using System.Reflection;

namespace DennisBlight.Modbus.Message
{
    public abstract class ModbusMessage
    {
        protected const byte FunctionCodeOffset = 0;

        private byte[] pdu;

        internal byte[] PDU => pdu;

        public int Length => pdu.Length;

        public FunctionCode FunctionCode
        {
            get
            {
                return GetType().GetCustomAttribute<FunctionCodeAttribute>().FunctionCode;
            }
        }

        /// <summary>Get the clone of underlying bytes of PDU segments.</summary>
        public virtual byte[] GetBytes() => (byte[])pdu.Clone();

        protected ModbusMessage(int baseLength)
        {
            pdu = new byte[baseLength];
            pdu[FunctionCodeOffset] = (byte)FunctionCode;
        }

        internal ModbusMessage(byte[] buffer)
        {
            try
            {
                if(false) CheckIntegrity(buffer);
                pdu = buffer;
            }
            catch(ConstraintViolationException)
            {
                throw new IntegrityViolationException();
            }
        }

        protected void ResizePdu(int newSize) => Array.Resize(ref pdu, newSize);

        protected abstract void CheckIntegrity(byte[] buffer);

        protected static void CheckConstraint(int value, int min, int max, string fieldName)
        {
            if (min <= value && value <= max) return;
            throw new ConstraintViolationException($"{fieldName} value must between {min} and {max}");
        }
    }

    public abstract class ModbusRequest : ModbusMessage
    {
        protected ModbusRequest(int baseLength) : base(baseLength) { }

        internal ModbusRequest(byte[] buffer) : base(buffer) { }

        /// <summary>Only check wether the function code is valid.</summary>
        protected override void CheckIntegrity(byte[] buffer)
        {
            if (buffer[0] != (byte)FunctionCode) throw new IntegrityViolationException();
        }

        public static ModbusRequest ParseBuffer(byte[] buffer)
        {
            FunctionCode functionCode = (FunctionCode)buffer[0];

            foreach (var t in Assembly.GetAssembly(typeof(ModbusMessage)).GetTypes())
            {
                if (t.IsSubclassOf(typeof(ModbusRequest)))
                {
                    var fcAttribute = t.GetCustomAttribute<FunctionCodeAttribute>();
                    if (fcAttribute != null && fcAttribute.FunctionCode == functionCode)
                    {
                        try
                        {
                            var ctor = t.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(byte[]) }, null);
                            return ctor.Invoke(new object[] { buffer }) as ModbusRequest;
                        }
                        catch (NullReferenceException)
                        {
                            throw new NullReferenceException($"Inaccessible constructor {t.Name}(byte[])");
                        }
                    }
                }
            }
            throw new NotImplementedException($"Request for function code 0x{buffer[0]:X2} is not implemented or not supported");
        }
    }

    public abstract class ModbusResponse : ModbusMessage
    {
        protected const byte ExceptionCodeOffset = 1;
        protected const byte ExceptionResponseBaseLength = 2;

        public bool HasException => (PDU[0] & 0x80) == 0x80;

        public ExceptionCode ExceptionCode => (ExceptionCode) (HasException ? PDU[ExceptionCodeOffset] : 0);

        protected ModbusResponse(int baseLength)
            : base(baseLength)
        { }

        internal ModbusResponse(byte[] buffer) : base(buffer) { }

        protected ModbusResponse(ExceptionCode code)
            : base(ExceptionResponseBaseLength)
        {
            PDU[FunctionCodeOffset] |= 0x80;
            PDU[ExceptionCodeOffset] = (byte)code;
        }

        /// <summary>Only check wether the function code is valid.</summary>
        protected override void CheckIntegrity(byte[] buffer)
        {
            if ((buffer[0] & 0x7f) != (byte)FunctionCode) throw new IntegrityViolationException();
            if(HasException && buffer.Length != 2) throw new IntegrityViolationException();
        }
        
        public static ModbusResponse ParseBuffer(byte[] buffer)
        {
            FunctionCode functionCode = (FunctionCode)(buffer[0] & 0x7f);

            foreach (var t in Assembly.GetAssembly(typeof(ModbusMessage)).GetTypes())
            {
                if (t.IsSubclassOf(typeof(ModbusResponse)))
                {
                    var fcAttribute = t.GetCustomAttribute<FunctionCodeAttribute>();
                    if (fcAttribute != null && fcAttribute.FunctionCode == functionCode)
                    {
                        try
                        {
                            var ctor = t.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(byte[]) }, null);
                            return ctor.Invoke(new object[] { buffer }) as ModbusResponse;
                        }
                        catch (NullReferenceException)
                        {
                            throw new NullReferenceException($"Inaccessible constructor {t.Name}(byte[])");
                        }
                    }
                }
            }
            throw new NotImplementedException($"Response for function code 0x{buffer[0]:X2} is not implemented or not supported");
        }
    }
    
    namespace Base
    {
        public abstract class ReadRequest : ModbusRequest
        {
            protected const byte StartingAddressOffset = 1;
            protected const byte QuantityOffset = 3;
            protected const byte BaseLength = 5;

            public ushort StartingAddress => BitHelper.ToUInt16(PDU, StartingAddressOffset);

            /// <summary>Read quantity</summary>
            public ushort Quantity => BitHelper.ToUInt16(PDU, QuantityOffset);

            internal ReadRequest(ushort address, ushort quantity)
                : base(BaseLength)
            {
                CheckQuantityConstraint(quantity);
                BitHelper.WriteBuffer(PDU, address, StartingAddressOffset);
                BitHelper.WriteBuffer(PDU, quantity, QuantityOffset);
            }

            internal ReadRequest(byte[] buffer) : base(buffer) { }

            protected sealed override void CheckIntegrity(byte[] buffer)
            {
                base.CheckIntegrity(buffer);
                if (buffer.Length != 5) throw new IntegrityViolationException();
                CheckQuantityConstraint(BitHelper.ToUInt16(buffer, QuantityOffset));
            }

            protected abstract void CheckQuantityConstraint(ushort quantity);
        }

        public abstract class WriteSingleRequest : ModbusRequest
        {
            protected const byte AddressOffset = 1;
            protected const byte ValueOffset = 3;
            protected const byte BaseLength = 5;

            /// <summary>Address or Starting Address of modbus message data parameter.</summary>
            public ushort Address => BitHelper.ToUInt16(PDU, AddressOffset);

            /// <summary>Written value</summary>
            public ushort Value => BitHelper.ToUInt16(PDU, ValueOffset);

            internal WriteSingleRequest(ushort address, ushort value)
                : base(BaseLength)
            {
                BitHelper.WriteBuffer(PDU, address, AddressOffset);
                BitHelper.WriteBuffer(PDU, value, ValueOffset);
            }

            internal WriteSingleRequest(byte[] buffer) : base(buffer) { }

            protected sealed override void CheckIntegrity(byte[] buffer)
            {
                base.CheckIntegrity(buffer);
                if (buffer.Length != 5) throw new IntegrityViolationException();
            }
        }

        public abstract class WriteMultiRequest : ModbusRequest
        {
            protected const byte StartingAddressOffset = 1;
            protected const byte QuantityOffset = 3;
            protected const byte ByteCountOffset = 5;
            protected const byte BaseLength = 6;

            private byte[] rawValues;
            private bool changed;

            public ushort StartingAddress => BitHelper.ToUInt16(PDU, StartingAddressOffset);

            /// <summary>Written values quantity</summary>
            public ushort Quantity => BitHelper.ToUInt16(PDU, QuantityOffset);

            /// <summary>Byte length of written values.</summary>
            public byte ByteCount => PDU[ByteCountOffset];

            /// <summary>
            /// Get or set bytes of value segments.
            /// Don't change underlying value directly (via indexing) as it wouldn't invoke 'changed' field.
            /// Set the whole bytes instead (via assignment).
            /// </summary>
            protected byte[] RawValues
            {
                get { return rawValues; }
                set
                {
                    CheckByteCountConstraint(value.Length);
                    rawValues = value;
                    PDU[ByteCountOffset] = (byte)value.Length;
                    changed = true;
                }
            }

            /// <summary>Get the clone of underlying bytes of PDU segments.</summary>
            public override byte[] GetBytes()
            {
                if (changed)
                {
                    if (PDU.Length != (BaseLength + RawValues.Length))
                    {
                        ResizePdu(BaseLength + RawValues.Length);
                    }
                    Buffer.BlockCopy(RawValues, 0, PDU, BaseLength, RawValues.Length);
                    changed = false;
                }
                return base.GetBytes();
            }

            internal WriteMultiRequest(ushort address)
                : base(BaseLength)
            {
                BitHelper.WriteBuffer(PDU, address, StartingAddressOffset);
                changed = true;
            }

            internal WriteMultiRequest(byte[] buffer) : base(buffer)
            {
                changed = false;
            }

            protected sealed override void CheckIntegrity(byte[] buffer)
            {
                base.CheckIntegrity(buffer);
                CheckQuantityConstraint(Quantity);
                if (ByteCount != (buffer.Length - BaseLength)) throw new IntegrityViolationException();
            }

            protected abstract void CheckQuantityConstraint(ushort quantity);
            protected abstract void CheckByteCountConstraint(int byteCount);
        }

        public abstract class ReadResponse : ModbusResponse
        {
            protected const byte ByteCountOffset = 1;
            protected const byte BaseLength = 2;

            private byte[] rawValues;
            private bool changed = true;

            /// <summary>Byte length of readed values.</summary>
            public byte ByteCount => PDU[ByteCountOffset];

            /// <summary>
            /// Get or set bytes of value segments.
            /// Don't change underlying value directly (via indexing) as it wouldn't invoke 'changed' field.
            /// Set the whole bytes instead (via assignment).
            /// </summary>
            protected byte[] RawValues
            {
                get { return rawValues; }
                set
                {
                    CheckByteCountConstraint(value.Length);
                    rawValues = value;
                    PDU[ByteCountOffset] = (byte)value.Length;
                    changed = true;
                }
            }

            /// <summary>Get the clone of underlying bytes of PDU segments.</summary>
            public override byte[] GetBytes()
            {
                if (changed)
                {
                    if (PDU.Length != (BaseLength + RawValues.Length))
                    {
                        ResizePdu(BaseLength + RawValues.Length);
                    }
                    Buffer.BlockCopy(RawValues, 0, PDU, BaseLength, RawValues.Length);
                    changed = false;
                }
                return base.GetBytes();
            }

            protected ReadResponse(ExceptionCode code)
                : base(code)
            { }

            internal ReadResponse()
                : base(BaseLength)
            {
                changed = true;
            }

            internal ReadResponse(byte[] buffer) : base(buffer) { }

            protected override void CheckIntegrity(byte[] buffer)
            {
                base.CheckIntegrity(buffer);
                if(!HasException)
                {
                    if (ByteCount != (buffer.Length - BaseLength)) throw new IntegrityViolationException();
                }
            }

            protected abstract void CheckByteCountConstraint(int value);
        }

        public abstract class WriteSingleResponse : ModbusResponse
        {
            protected const byte AddressOffset = 1;
            protected const byte ValueOffset = 3;
            protected const byte BaseLength = 5;

            public ushort Address => BitHelper.ToUInt16(PDU, AddressOffset);

            public ushort Value => BitHelper.ToUInt16(PDU, ValueOffset);

            internal WriteSingleResponse(ushort address, ushort value)
                : base(BaseLength)
            {
                BitHelper.WriteBuffer(PDU, address, AddressOffset);
                BitHelper.WriteBuffer(PDU, value, ValueOffset);
            }

            internal WriteSingleResponse(byte[] buffer) : base(buffer) { }

            protected sealed override void CheckIntegrity(byte[] buffer)
            {
                base.CheckIntegrity(buffer);
                if(!HasException)
                {
                    if (buffer.Length != BaseLength) throw new IntegrityViolationException();
                }
            }

            protected WriteSingleResponse(ExceptionCode code)
                : base(code)
            { }
        }

        public abstract class WriteMultiResponse : ModbusResponse
        {
            protected const byte AddressOffset = 1;
            protected const byte QuantityOffset = 3;
            protected const byte BaseLength = 5;

            public ushort Address => BitHelper.ToUInt16(PDU, AddressOffset);

            public ushort Quantity => BitHelper.ToUInt16(PDU, QuantityOffset);

            internal WriteMultiResponse(ushort address, ushort quantity)
                : base(BaseLength)
            {
                CheckQuantityConstraint(quantity);
                BitHelper.WriteBuffer(PDU, address, AddressOffset);
                BitHelper.WriteBuffer(PDU, quantity, QuantityOffset);
            }

            protected WriteMultiResponse(ExceptionCode code)
                : base(code)
            { }

            internal WriteMultiResponse(byte[] buffer) : base(buffer) { }

            protected sealed override void CheckIntegrity(byte[] buffer)
            {
                base.CheckIntegrity(buffer);
                if(!HasException)
                {
                    if (buffer.Length != BaseLength) throw new IntegrityViolationException();
                    CheckQuantityConstraint(Quantity);
                }
            }

            protected abstract void CheckQuantityConstraint(ushort quantity);
        }
    }
}
