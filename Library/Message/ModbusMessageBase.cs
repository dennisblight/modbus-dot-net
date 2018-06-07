﻿using System;

namespace DennisBlight.Modbus.Message
{
    public abstract class ModbusMessage
    {
        protected const int FunctionCodeOffset = 0;

        private byte[] pdu;

        protected byte[] PDU => pdu;

        /// <summary>Return the function code for this message. This value wouldn't expose exception flags.</summary>
        public abstract FunctionCode FunctionCode { get; }

        /// <summary>Get the clone of underlying bytes of PDU segments.</summary>
        public virtual byte[] GetBytes() => (byte[])pdu.Clone();

        protected ModbusMessage(int baseLength)
        {
            pdu = new byte[baseLength];
            pdu[FunctionCodeOffset] = (byte)FunctionCode;
        }

        protected ModbusMessage(byte[] buffer)
        {
            CheckIntegrity(buffer);
            pdu = buffer;
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
        protected const int AddressOffset = 1;

        /// <summary>Address or Starting Address of modbus message data parameter.</summary>
        public ushort Address => BitHelper.ToUInt16(PDU, AddressOffset);

        protected ModbusRequest(int baseLength, ushort address)
            : base(baseLength)
        {
            BitHelper.WriteBuffer(PDU, address, AddressOffset);
        }

        /// <summary>Only check wether the function code is valid.</summary>
        protected override void CheckIntegrity(byte[] buffer)
        {
            if (buffer[0] != (byte)FunctionCode) throw new IntegrityViolationException();
        }
    }

    public abstract class ModbusResponse : ModbusMessage
    {
        protected const int ExceptionCodeOffset = 1;
        protected const int ExceptionResponseBaseLength = 2;

        public bool HasException => (PDU[0] & 0x80) == 0x80;

        public ExceptionCode ExceptionCode => (ExceptionCode) (HasException ? PDU[ExceptionCodeOffset] : 0);

        protected ModbusResponse(int baseLength)
            : base(baseLength)
        { }

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
    }
    
    namespace Base
    {
        public abstract class ReadRequest : ModbusRequest
        {
            protected const int QuantityOffset = 3;
            protected const int BaseLength = 5;

            /// <summary>Read quantity</summary>
            public ushort Quantity => BitHelper.ToUInt16(PDU, QuantityOffset);

            internal ReadRequest(ushort address, ushort quantity)
                : base(BaseLength, address)
            {
                CheckQuantityConstraint(quantity);
                BitHelper.WriteBuffer(PDU, quantity, QuantityOffset);
            }

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
            protected const int ValueOffset = 3;
            protected const int BaseLength = 5;

            /// <summary>Written value</summary>
            public ushort Value => BitHelper.ToUInt16(PDU, ValueOffset);

            internal WriteSingleRequest(ushort address, ushort value)
                : base(BaseLength, address)
            {
                BitHelper.WriteBuffer(PDU, value, ValueOffset);
            }

            protected sealed override void CheckIntegrity(byte[] buffer)
            {
                base.CheckIntegrity(buffer);
                if (buffer.Length != 5) throw new IntegrityViolationException();
            }
        }

        public abstract class WriteMultiRequest : ModbusRequest
        {
            protected const int QuantityOffset = 3;
            protected const int ByteCountOffset = 5;
            protected const int BaseLength = 6;

            private byte[] rawValues;
            private bool changed;

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
                : base(BaseLength, address)
            {
                changed = true;
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
            protected const int ByteCountOffset = 1;
            protected const int BaseLength = 2;

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

            internal ReadResponse()
                : base(BaseLength)
            {
                changed = true;
            }

            protected ReadResponse(ExceptionCode code)
                : base(code)
            { }

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
            protected const int AddressOffset = 1;
            protected const int ValueOffset = 3;
            protected const int BaseLength = 5;

            public ushort Address => BitHelper.ToUInt16(PDU, AddressOffset);

            public ushort Value => BitHelper.ToUInt16(PDU, ValueOffset);

            internal WriteSingleResponse(ushort address, ushort value)
                : base(BaseLength)
            {
                BitHelper.WriteBuffer(PDU, address, AddressOffset);
                BitHelper.WriteBuffer(PDU, value, ValueOffset);
            }

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
            protected const int AddressOffset = 1;
            protected const int QuantityOffset = 3;
            protected const int BaseLength = 5;

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
