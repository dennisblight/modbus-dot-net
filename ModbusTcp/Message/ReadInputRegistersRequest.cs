using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public class ReadInputRegistersRequest : ModbusMessage
    {
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
            set
            {
                CheckConstraint(value, 1, 0x7d, nameof(Quantity));
                BitHelper.WriteBuffer(Adu, (ushort)value, BaseSize + 2);
            }
        }

        public ReadInputRegistersRequest()
            : base(FunctionCode.ReadInputRegisters)
        {
            Array.Resize(ref Adu, BaseSize + 4);
        }
    }
}
