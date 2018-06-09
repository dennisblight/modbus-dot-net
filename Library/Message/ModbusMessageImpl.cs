using DennisBlight.Modbus.Message.Base;

namespace DennisBlight.Modbus.Message
{
    public class ReadCoilsRequest : ReadRequest
    {
        public ReadCoilsRequest(ushort address, ushort quantity)
            : base(address, quantity)
        { }

        internal ReadCoilsRequest(byte[] buffer) : base(buffer) { }

        protected override void CheckQuantityConstraint(ushort quantity)
        {
            CheckConstraint(quantity, 1, 2000, nameof(Quantity));
        }

        public override FunctionCode FunctionCode => FunctionCode.ReadCoils;
    }

    public class ReadDiscreteInputsRequest : ReadCoilsRequest
    {
        public ReadDiscreteInputsRequest(ushort address, ushort quantity)
            : base(address, quantity)
        { }

        internal ReadDiscreteInputsRequest(byte[] buffer) : base(buffer) { }

        public override FunctionCode FunctionCode => FunctionCode.ReadDiscreteInputs;
    }

    public class ReadHoldingRegistersRequest : ReadRequest
    {
        public ReadHoldingRegistersRequest(ushort address, ushort quantity)
            : base(address, quantity)
        { }

        internal ReadHoldingRegistersRequest(byte[] buffer) : base(buffer) { }

        protected override void CheckQuantityConstraint(ushort quantity)
        {
            CheckConstraint(quantity, 1, 125, nameof(Quantity));
        }

        public override FunctionCode FunctionCode => FunctionCode.ReadHoldingRegisters;
    }

    public class ReadInputRegistersRequest : ReadHoldingRegistersRequest
    {
        public ReadInputRegistersRequest(ushort address, ushort quantity)
            : base(address, quantity)
        { }

        internal ReadInputRegistersRequest(byte[] buffer) : base(buffer) { }

        public override FunctionCode FunctionCode => FunctionCode.ReadInputRegisters;
    }

    public class ReadCoilsResponse : ReadResponse
    {
        public bool[] GetBitStatus()
        {
            bool[] bits = new bool[8 * ByteCount];

            for(int i = 0; i < RawValues.Length; i++)
            {
                byte b = RawValues[i];
                for(int j = 0; j < 8; j++)
                {
                    bits[i + 8 * j] = ((b >> j) & 0x01) == 0x01;
                }
            }

            return bits;
        }
        
        public ReadCoilsResponse(ExceptionCode code)
            : base(code)
        { }

        public ReadCoilsResponse(params bool[] bits)
            : base()
        {
            byte[] bytes = new byte[(bits.Length / 8) + (bits.Length % 8 > 0 ? 1 : 0)];

            byte b = 0;
            int k = 0;
            // i = iterator, j = shifter, k = indexer
            for (int i = 0, j = 0; i < bits.Length; i++, j++)
            {
                if (j == 8)
                {
                    bytes[k++] = b;
                    j = 0;
                    b = 0;
                }
                if (bits[i])
                {
                    b |= (byte)(1 << j);
                }
            }
            bytes[k] = b;
            RawValues = bytes;
        }

        internal ReadCoilsResponse(byte[] buffer) : base(buffer) { }

        protected override void CheckByteCountConstraint(int value)
        {
            CheckConstraint(value, 1, 250, nameof(ByteCount));
        }

        public override FunctionCode FunctionCode => FunctionCode.ReadCoils;
    }

    public class ReadDiscreteInputsResponse : ReadCoilsResponse
    {
        public ReadDiscreteInputsResponse(params bool[] bits)
            : base(bits)
        { }

        public ReadDiscreteInputsResponse(ExceptionCode code)
            : base(code)
        { }

        internal ReadDiscreteInputsResponse(byte[] buffer) : base(buffer) { }

        public override FunctionCode FunctionCode => FunctionCode.ReadDiscreteInputs;
    }

    public class ReadHoldingRegistersResponse : ReadResponse
    {
        public ushort[] GetRegisterValues()
        {
            ushort[] registers = new ushort[ByteCount / 2];

            for(int i = 0; i < RawValues.Length; i++)
            {
                registers[i] = BitHelper.ToUInt16(RawValues, 2 * i);
            }

            return registers;
        }

        public ReadHoldingRegistersResponse(params ushort[] registers)
            : base()
        {
            byte[] bytes = new byte[2 * registers.Length];

            for(int i = 0; i < registers.Length; i++)
            {
                BitHelper.WriteBuffer(bytes, registers[i], 2 * i);
            }

            RawValues = bytes;
        }

        public ReadHoldingRegistersResponse(ExceptionCode code)
            : base(code)
        { }

        internal ReadHoldingRegistersResponse(byte[] buffer) : base(buffer) { }

        protected override void CheckByteCountConstraint(int value)
        {
            CheckConstraint(value, 1, 250, nameof(ByteCount));
        }

        public override FunctionCode FunctionCode => FunctionCode.ReadHoldingRegisters;
    }

    public class ReadInputRegistersResponse : ReadHoldingRegistersResponse
    {
        public ReadInputRegistersResponse(params ushort[] registers)
            : base(registers)
        { }

        public ReadInputRegistersResponse(ExceptionCode code)
            : base(code)
        { }

        internal ReadInputRegistersResponse(byte[] buffer) : base(buffer) { }

        public override FunctionCode FunctionCode => FunctionCode.ReadInputRegisters;
    }

    public class WriteSingleCoilRequest : WriteSingleRequest
    {
        public bool BitStatus => Value > 0;

        public WriteSingleCoilRequest(ushort address, bool value) 
            : base(address, (ushort)(value ? 0xff00 : 0))
        { }

        internal WriteSingleCoilRequest(byte[] buffer) : base(buffer) { }

        public override FunctionCode FunctionCode => FunctionCode.WriteSingleCoil;
    }

    public class WriteSingleRegisterRequest : WriteSingleRequest
    {
        public WriteSingleRegisterRequest(ushort address, ushort value)
            : base(address, value)
        { }

        internal WriteSingleRegisterRequest(byte[] buffer) : base(buffer) { }

        public override FunctionCode FunctionCode => FunctionCode.WriteSingleRegister;
    }

    public class WriteSingleCoilResponse : WriteSingleResponse
    {
        public bool BitStatus => Value > 0;

        public WriteSingleCoilResponse(ushort address, bool value)
            : base(address, (ushort)(value ? 0xff00 : 0))
        { }

        public WriteSingleCoilResponse(ExceptionCode code)
            : base(code)
        { }

        internal WriteSingleCoilResponse(byte[] buffer) : base(buffer) { }

        public override FunctionCode FunctionCode => FunctionCode.WriteSingleCoil;
    }

    public class WriteSingleRegisterResponse : WriteSingleResponse
    {
        public WriteSingleRegisterResponse(ushort address, ushort value)
            : base(address, value)
        { }

        public WriteSingleRegisterResponse(ExceptionCode code)
            : base(code)
        { }

        internal WriteSingleRegisterResponse(byte[] buffer) : base(buffer) { }

        public override FunctionCode FunctionCode => FunctionCode.WriteSingleRegister;
    }

    public class WriteMultipleCoilsRequest : WriteMultiRequest
    {
        public bool[] GetBitStatus()
        {
            bool[] bits = new bool[8 * ByteCount];

            for (int i = 0; i < RawValues.Length; i++)
            {
                byte b = RawValues[i];
                for (int j = 0; j < 8; j++)
                {
                    bits[i + 8 * j] = ((b >> j) & 0x01) == 0x01;
                }
            }

            return bits;
        }

        public WriteMultipleCoilsRequest(ushort address, params bool[] values)
            : base(address)
        {
            byte[] bytes = new byte[(values.Length / 8) + (values.Length % 8 > 0 ? 1 : 0)];

            byte b = 0;
            int k = 0;
            // i = iterator, j = shifter, k = indexer
            for (int i = 0, j = 0; i < values.Length; i++, j++)
            {
                if (j == 8)
                {
                    bytes[k++] = b;
                    j = 0;
                    b = 0;
                }
                if (values[i])
                {
                    b |= (byte)(1 << j);
                }
            }
            bytes[k] = b;
            RawValues = bytes;
            BitHelper.WriteBuffer(PDU, (ushort)values.Length, QuantityOffset);
        }

        internal WriteMultipleCoilsRequest(byte[] buffer) : base(buffer) { }

        protected override void CheckByteCountConstraint(int byteCount)
        {
            CheckConstraint(byteCount, 1, 246, nameof(ByteCount));
        }

        protected override void CheckQuantityConstraint(ushort quantity)
        {
            CheckConstraint(quantity, 1, 1968, nameof(Quantity));
        }

        public override FunctionCode FunctionCode => FunctionCode.WriteMultipleCoils;
    }

    public class WriteMultipleRegistersRequest : WriteMultiRequest
    {
        public ushort[] GetRegisterValues()
        {
            ushort[] registers = new ushort[ByteCount / 2];

            for (int i = 0; i < RawValues.Length; i++)
            {
                registers[i] = BitHelper.ToUInt16(RawValues, 2 * i);
            }

            return registers;
        }

        public WriteMultipleRegistersRequest(ushort address, params ushort[] values)
            : base(address)
        {
            byte[] bytes = new byte[2 * values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                BitHelper.WriteBuffer(bytes, values[i], 2 * i);
            }

            RawValues = bytes;
            BitHelper.WriteBuffer(PDU, (ushort)values.Length, QuantityOffset);
        }

        internal WriteMultipleRegistersRequest(byte[] buffer) : base(buffer) { }

        protected override void CheckByteCountConstraint(int byteCount)
        {
            CheckConstraint(byteCount, 1, 246, nameof(ByteCount));
        }

        protected override void CheckQuantityConstraint(ushort quantity)
        {
            CheckConstraint(quantity, 1, 123, nameof(Quantity));
        }

        public override FunctionCode FunctionCode => FunctionCode.WriteMultipleRegisters;
    }

    public class WriteMultipleCoilsResponse : WriteMultiResponse
    {
        public WriteMultipleCoilsResponse(ushort address, ushort quantity)
            : base(address, quantity)
        { }

        public WriteMultipleCoilsResponse(ExceptionCode code)
            : base(code)
        { }

        internal WriteMultipleCoilsResponse(byte[] buffer) : base(buffer) { }

        protected override void CheckQuantityConstraint(ushort quantity)
        {
            CheckConstraint(quantity, 1, 1968, nameof(Quantity));
        }

        public override FunctionCode FunctionCode => FunctionCode.WriteMultipleCoils;
    }

    public class WriteMultipleRegistersResponse : WriteMultiResponse
    {
        public WriteMultipleRegistersResponse(ushort address, ushort quantity)
            : base(address, quantity)
        { }

        public WriteMultipleRegistersResponse(ExceptionCode code)
            : base(code)
        { }

        internal WriteMultipleRegistersResponse(byte[] buffer) : base(buffer) { }

        protected override void CheckQuantityConstraint(ushort quantity)
        {
            CheckConstraint(quantity, 1, 123, nameof(Quantity));
        }

        public override FunctionCode FunctionCode => FunctionCode.WriteMultipleRegisters;
    }
}
