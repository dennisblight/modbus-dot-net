using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public class WriteSingleCoilRequest : ModbusMessage
    {
        public int OutputAddress
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 0); }
            set
            {
                CheckConstraint(value, 0, 0xffff, nameof(OutputAddress));
                BitHelper.WriteBuffer(Adu, (ushort)value, BaseSize + 0);
            }
        }

        public bool OutputValue
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 2) != 0; }
            set
            {
                BitHelper.WriteBuffer(Adu, (ushort)(value ? 0xff00 : 0x00), BaseSize + 2);
            }
        }

        public WriteSingleCoilRequest() 
            : base(FunctionCode.WriteSingleCoil)
        {
            Array.Resize(ref Adu, BaseSize + 4);
        }
    }
}
