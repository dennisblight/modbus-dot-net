using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public class WriteMultipleRegistersRequest : ModbusMessage
    {
        private byte[] registersValue;

        public int StartingIndex
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 0); }
            set
            {
                CheckConstraint(value, 0, 0xffff, nameof(StartingIndex));
                BitHelper.WriteBuffer(Adu, (ushort)value, BaseSize + 0);
            }
        }

        public int Quantity
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 2); }
            private set
            {
                CheckConstraint(value, 1, 0x7b, nameof(Quantity));
                BitHelper.WriteBuffer(Adu, (ushort)value, BaseSize + 2);
            }
        }

        public int ByteCount
        {
            get { return Adu[BaseSize + 4]; }
            private set { Adu[BaseSize + 4] = (byte)value; }
        }

        public void SetRegistersValue(params ushort[] values)
        {
            byte[] bytes = new byte[Buffer.ByteLength(values)];
            for(int i = 0; i < values.Length; i++)
            {
                BitHelper.WriteBuffer(bytes, values[i], 2 * i);
            }
            ByteCount = bytes.Length;
            Quantity = values.Length;
            registersValue = bytes;
            Array.Resize(ref Adu, BaseSize + 5 + bytes.Length);
            Buffer.BlockCopy(bytes, 0, Adu, BaseSize + 5, bytes.Length);
        }

        public byte[] GetRegistersValueBytes()
        {
            return (byte[])registersValue.Clone();
        }

        public ushort[] GetRegistersValue()
        {
            ushort[] registers = new ushort[registersValue.Length / 2];
            for(int i = 0; i < registers.Length; i++)
            {
                registers[i] = BitHelper.ToUInt16(registersValue, i * 2);
            }
            return registers;
        }

        public WriteMultipleRegistersRequest() 
            : base(FunctionCode.WriteMultipleRegisters)
        {
            Array.Resize(ref Adu, BaseSize + 5);
        }
    }
}
