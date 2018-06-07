using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DennisBlight.Modbus.Message
{
    public abstract class ModbusMessage
    {
        protected const int FunctionCodeOffset = 0;

        private byte[] pdu;

        protected byte[] PDU
        {
            get { return pdu; }
        }

        /// <summary>Return the function code for this message. This value wouldn't expose exception flags.</summary>
        public FunctionCode FunctionCode
        {
            get { return (FunctionCode)(pdu[FunctionCodeOffset] & 0x7f); }
        }

        /// <summary>Get the clone of underlying bytes of PDU segments.</summary>
        public virtual byte[] GetBytes()
        {
            return (byte[])pdu.Clone();
        }

        protected ModbusMessage(FunctionCode functionCode, int baseLength)
        {
            pdu = new byte[baseLength];
            pdu[FunctionCodeOffset] = (byte)functionCode;
        }

        protected internal void ResizePdu(int newSize)
        {
            Array.Resize(ref pdu, newSize);
        }

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
        public ushort Address
        {
            get { return BitHelper.ToUInt16(PDU, AddressOffset); }
        }

        protected ModbusRequest(FunctionCode functionCode, int baseLength, ushort address)
            : base(functionCode, baseLength)
        {
            BitHelper.WriteBuffer(PDU, address, AddressOffset);
        }
    }

    public abstract class ModbusResponse : ModbusMessage
    {
        protected const int ExceptionCodeOffset = 1;

        public bool HasException
        {
            get { return (PDU[0] & 0x80) == 0x80; }
        }

        public ExceptionCode ExceptionCode
        {
            get { return (ExceptionCode)(HasException ? PDU[ExceptionCodeOffset] : 0); }
        }

        protected ModbusResponse(FunctionCode functionCode, int baseLength)
            : base(functionCode, baseLength)
        { }

        protected ModbusResponse(FunctionCode functionCode, ExceptionCode code)
            : base(functionCode, 2)
        {
            PDU[ExceptionCodeOffset] = (byte)code;
        }
    }
    
    namespace Base
    {
        public abstract class ReadRequest : ModbusRequest
        {
            protected const int QuantityOffset = 3;

            /// <summary>Read quantity</summary>
            public ushort Quantity
            {
                get { return BitHelper.ToUInt16(PDU, QuantityOffset); }
            }

            internal ReadRequest(FunctionCode functionCode, ushort address, ushort quantity)
                : base(functionCode, 5, address)
            {
                CheckQuantityConstraint(quantity);
                BitHelper.WriteBuffer(PDU, quantity, QuantityOffset);
            }

            protected abstract void CheckQuantityConstraint(ushort quantity);
        }

        public abstract class WriteSingleRequest : ModbusRequest
        {
            protected const int ValueOffset = 3;

            /// <summary>Written value</summary>
            public ushort Value
            {
                get { return BitHelper.ToUInt16(PDU, ValueOffset); }
            }

            internal WriteSingleRequest(FunctionCode functionCode, ushort address, ushort value)
                : base(functionCode, 5, address)
            {
                BitHelper.WriteBuffer(PDU, value, ValueOffset);
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
            public ushort Quantity
            {
                get { return BitHelper.ToUInt16(PDU, QuantityOffset); }
            }

            /// <summary>Byte length of written values.</summary>
            public byte ByteCount
            {
                get { return PDU[ByteCountOffset]; }
            }

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

            internal WriteMultiRequest(FunctionCode functionCode, ushort address)
                : base(functionCode, BaseLength, address)
            {
                changed = true;
            }

            protected abstract void CheckQuantityConstraint(ushort quantity);
            protected abstract void CheckByteCountConstraint(byte byteCount);
        }

        public abstract class ReadResponse : ModbusResponse
        {
            protected const int ByteCountOffset = 1;
            protected const int BaseLength = 2;

            private byte[] rawValues;
            private bool changed = true;

            /// <summary>Byte length of readed values.</summary>
            public byte ByteCount
            {
                get { return PDU[ByteCountOffset]; }
            }

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

            internal ReadResponse(FunctionCode functionCode)
                : base(functionCode, BaseLength)
            {
                changed = true;
            }

            protected ReadResponse(FunctionCode functionCode, ExceptionCode code)
                : base(functionCode, code)
            { }

            protected abstract void CheckByteCountConstraint(byte value);
        }

        public abstract class WriteSingleResponse : ModbusResponse
        {
            protected const int AddressOffset = 1;
            protected const int ValueOffset = 3;

            public ushort Address
            {
                get { return BitHelper.ToUInt16(PDU, AddressOffset); }
            }

            public ushort Value
            {
                get { return BitHelper.ToUInt16(PDU, ValueOffset); }
            }

            internal WriteSingleResponse(FunctionCode functionCode, ushort address, ushort value)
                : base(functionCode, 5)
            {
                BitHelper.WriteBuffer(PDU, address, AddressOffset);
                BitHelper.WriteBuffer(PDU, value, ValueOffset);
            }

            protected WriteSingleResponse(FunctionCode functionCode, ExceptionCode code)
                : base(functionCode, code)
            { }
        }

        public abstract class WriteMultiResponse : ModbusResponse
        {
            protected const int AddressOffset = 1;
            protected const int QuantityOffset = 3;

            public ushort Address
            {
                get { return BitHelper.ToUInt16(PDU, AddressOffset); }
            }

            public ushort Quantity
            {
                get { return BitHelper.ToUInt16(PDU, QuantityOffset); }
            }

            internal WriteMultiResponse(FunctionCode functionCode, ushort address, ushort quantity)
                : base(functionCode, 5)
            {
                CheckQuantityConstraint(quantity);
                BitHelper.WriteBuffer(PDU, address, AddressOffset);
                BitHelper.WriteBuffer(PDU, quantity, QuantityOffset);
            }

            protected WriteMultiResponse(FunctionCode functionCode, ExceptionCode code) : base(functionCode, code)
            { }

            protected abstract void CheckQuantityConstraint(ushort quantity);
        }
    }
}
