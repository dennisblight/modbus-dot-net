using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public class WriteMultipleCoilsRequest : ModbusMessage
    {
        private byte[] outputsValue;

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
                CheckConstraint(value, 1, 0x7b0, nameof(Quantity));
                BitHelper.WriteBuffer(Adu, (ushort)value, BaseSize + 2);
            }
        }

        public int ByteCount
        {
            get { return Adu[BaseSize + 4]; }
            private set { Adu[BaseSize + 4] = (byte)value; }
        }

        public void SetOutputsValue(params bool[] values)
        {
            byte[] bytes = new byte[(values.Length / 8) + (values.Length % 8 > 0 ? 1 : 0)];
            int b = 0, k = 0;
            for (int i = 0, j = 0; i < values.Length; i++, j++)
            {
                if (j == 8)
                {
                    bytes[k++] = (byte)b;
                    j = 0;
                    b = 0;
                }
                if (values[i])
                {
                    b |= 1 << j;
                }
            }
            bytes[k] = (byte)b;
            SetOutputsValue(values.Length, bytes);
        }

        public void SetOutputsValue(params byte[] values)
        {
            SetOutputsValue(8 * values.Length, values);
        }

        public void SetOutputsValue(int quantity, params byte[] values)
        {
            if (quantity > values.Length * 8) quantity = values.Length * 8;
            int byteCount = (quantity / 8) + (quantity % 8 > 0 ? 1 : 0);
            if (values.Length != byteCount) Array.Resize(ref values, byteCount);
            ByteCount = byteCount;
            Quantity = quantity;
            outputsValue = values;
            Array.Resize(ref Adu, BaseSize + 5 + byteCount);
            Buffer.BlockCopy(values, 0, Adu, BaseSize + 5, byteCount);
        }

        public byte[] GetOutputsValueBytes()
        {
            return (byte[])outputsValue.Clone();
        }

        public bool[] GetOutputsValue()
        {
            bool[] values = new bool[Quantity];
            for(int i = 0, j = 0; i < outputsValue.Length; i++)
            {
                byte b = outputsValue[i];
                for(int k = 0; k < 8 && j < values.Length; k++, j++)
                {
                    values[j] = ((b >> k) & 0x01) == 1;
                }
            }
            return values;
        }

        public WriteMultipleCoilsRequest() 
            : base(FunctionCode.WriteMultipleCoils)
        {
            Array.Resize(ref Adu, BaseSize + 5);
        }
    }
}
