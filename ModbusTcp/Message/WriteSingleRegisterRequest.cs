using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public class WriteSingleRegisterRequest : ModbusMessage
    {
        public int RegisterAddress
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 0); }
            set
            {
                CheckConstraint(value, 0, 0xffff, nameof(RegisterAddress));
                BitHelper.WriteBuffer(Adu, (ushort)value, BaseSize + 0);
            }
        }

        public int RegisterValue
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 2); }
            set
            {
                CheckConstraint(value, 0, 0xffff, nameof(RegisterValue));
                BitHelper.WriteBuffer(Adu, (ushort)value, BaseSize + 2);
            }
        }

        public WriteSingleRegisterRequest() 
            : base(FunctionCode.WriteSingleRegister)
        {
            Array.Resize(ref Adu, BaseSize + 4);
        }
    }
}
