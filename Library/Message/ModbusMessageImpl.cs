using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DennisBlight.Modbus.Message.Base;

namespace DennisBlight.Modbus.Message
{
    public class ReadCoilsRequest : ReadRequest
    {
        public ReadCoilsRequest(ushort address, ushort quantity)
            : base(FunctionCode.ReadCoils, address, quantity)
        { }

        protected internal ReadCoilsRequest(FunctionCode functionCode, ushort address, ushort quantity)
            : base(functionCode, address, quantity)
        { }

        protected override void CheckQuantityConstraint(ushort quantity)
        {
            CheckConstraint(quantity, 1, 2000, nameof(Quantity));
        }
    }

    public class ReadDiscreteInputsRequest : ReadCoilsRequest
    {
        public ReadDiscreteInputsRequest(ushort address, ushort quantity) 
            : base(FunctionCode.ReadDiscreteInputs, address, quantity)
        { }
    }

    public class ReadHoldingRegistersRequest : ReadRequest
    {
        public ReadHoldingRegistersRequest(ushort address, ushort quantity)
            : base(FunctionCode.ReadHoldingRegisters, address, quantity)
        { }

        protected internal ReadHoldingRegistersRequest(FunctionCode functionCode, ushort address, ushort quantity)
            : base(functionCode, address, quantity)
        { }

        protected override void CheckQuantityConstraint(ushort quantity)
        {
            CheckConstraint(quantity, 1, 125, nameof(Quantity));
        }
    }

    public class ReadInputRegistersRequest : ReadHoldingRegistersRequest
    {
        public ReadInputRegistersRequest(ushort address, ushort quantity)
            : base(FunctionCode.ReadInputRegisters, address, quantity)
        { }
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

        public ReadCoilsResponse(params bool[] bits)
            : this(FunctionCode.ReadCoils, bits)
        { }

        public ReadCoilsResponse(ExceptionCode code)
            : base(FunctionCode.ReadCoils, code)
        { }

        protected internal ReadCoilsResponse(FunctionCode functionCode, bool[] bits)
            : base(functionCode)
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
                    b |= (byte)(1 >> j);
                }
            }
            bytes[k] = b;
            RawValues = bytes;
        }

        protected internal ReadCoilsResponse(FunctionCode functionCode, ExceptionCode code)
            : base(functionCode, code)
        { }

        protected override void CheckByteCountConstraint(byte value)
        {
            CheckConstraint(value, 1, 250, nameof(ByteCount));
        }
    }

    public class ReadDiscreteInputsResponse : ReadCoilsResponse
    {
        public ReadDiscreteInputsResponse(params bool[] bits)
            : base(FunctionCode.ReadDiscreteInputs, bits)
        { }

        public ReadDiscreteInputsResponse(ExceptionCode code)
            : base(FunctionCode.ReadDiscreteInputs, code)
        { }
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

        protected internal ReadHoldingRegistersResponse(FunctionCode functionCode, ushort[] registers)
            : base(functionCode)
        {
            byte[] bytes = new byte[2 * registers.Length];

            for(int i = 0; i < registers.Length; i++)
            {
                BitHelper.WriteBuffer(bytes, registers[i], 2 * i);
            }

            RawValues = bytes;
        }

        protected internal ReadHoldingRegistersResponse(FunctionCode functionCode, ExceptionCode code)
            : base(functionCode, code)
        { }

        public ReadHoldingRegistersResponse(ExceptionCode code)
            : base(FunctionCode.ReadHoldingRegisters, code)
        { }

        public ReadHoldingRegistersResponse(params ushort[] registers)
            : this(FunctionCode.ReadHoldingRegisters, registers)
        { }

        protected override void CheckByteCountConstraint(byte value)
        {
            CheckConstraint(value, 1, 250, nameof(ByteCount));
        }
    }

    public class ReadInputRegistersResponse : ReadHoldingRegistersResponse
    {
        public ReadInputRegistersResponse(params ushort[] registers)
            : base(FunctionCode.ReadInputRegisters, registers)
        { }

        public ReadInputRegistersResponse(ExceptionCode code)
            : base(FunctionCode.ReadInputRegisters, code)
        { }
    }

    public class WriteSingleCoilRequest : WriteSingleRequest
    {
        public bool BitStatus
        {
            get { return Value > 0; }
        }

        public WriteSingleCoilRequest(ushort address, bool value) 
            : base(FunctionCode.WriteSingleCoil, address, (ushort)(value ? 0xff00 : 0))
        { }
    }

    public class WriteSingleRegisterRequest : WriteSingleRequest
    {
        public WriteSingleRegisterRequest(ushort address, ushort value)
            : base(FunctionCode.WriteSingleRegister, address, value)
        { }
    }

    public class WriteSingleCoilResponse : WriteSingleResponse
    {
        public bool BitStatus
        {
            get { return Value > 0; }
        }

        public WriteSingleCoilResponse(ushort address, bool value)
            : base(FunctionCode.WriteSingleCoil, address, (ushort)(value ? 0xffff : 0))
        { }

        public WriteSingleCoilResponse(ExceptionCode code)
            : base(FunctionCode.WriteSingleCoil, code)
        { }
    }

    public class WriteSingleRegisterResponse : WriteSingleResponse
    {
        public WriteSingleRegisterResponse(ushort address, ushort value)
            : base(FunctionCode.WriteSingleRegister, address, value)
        { }

        public WriteSingleRegisterResponse(ExceptionCode code)
            : base(FunctionCode.WriteSingleRegister, code)
        { }
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
            : base(FunctionCode.WriteMultipleCoils, address)
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
                    b |= (byte)(1 >> j);
                }
            }
            bytes[k] = b;
            RawValues = bytes;
        }

        protected override void CheckByteCountConstraint(byte byteCount)
        {
            CheckConstraint(byteCount, 1, 246, nameof(ByteCount));
        }

        protected override void CheckQuantityConstraint(ushort quantity)
        {
            CheckConstraint(quantity, 1, 1968, nameof(Quantity));
        }
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
            : base(FunctionCode.WriteMultipleRegisters, address)
        {
            byte[] bytes = new byte[2 * values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                BitHelper.WriteBuffer(bytes, values[i], 2 * i);
            }

            RawValues = bytes;
        }

        protected override void CheckByteCountConstraint(byte byteCount)
        {
            CheckConstraint(byteCount, 1, 246, nameof(ByteCount));
        }

        protected override void CheckQuantityConstraint(ushort quantity)
        {
            CheckConstraint(quantity, 1, 123, nameof(Quantity));
        }
    }

    public class WriteMultipleCoilsResponse : WriteMultiResponse
    {
        public WriteMultipleCoilsResponse(ushort address, ushort quantity)
            : base(FunctionCode.WriteMultipleCoils, address, quantity)
        { }

        public WriteMultipleCoilsResponse(ExceptionCode code)
            : base(FunctionCode.WriteMultipleCoils, code)
        { }

        protected override void CheckQuantityConstraint(ushort quantity)
        {
            CheckConstraint(quantity, 1, 1968, nameof(Quantity));
        }
    }

    public class WriteMultipleRegistersResponse : WriteMultiResponse
    {
        public WriteMultipleRegistersResponse(ushort address, ushort quantity)
            : base(FunctionCode.WriteMultipleRegisters, address, quantity)
        { }

        public WriteMultipleRegistersResponse(ExceptionCode code)
            : base(FunctionCode.WriteMultipleRegisters, code)
        { }

        protected override void CheckQuantityConstraint(ushort quantity)
        {
            CheckConstraint(quantity, 1, 123, nameof(Quantity));
        }
    }
}
