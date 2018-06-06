using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public class WriteMultipleRegistersResponse : Response
    {
        public ushort StartingAddress
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 0); }
        }

        public ushort Quantity
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 2); }
        }

        internal WriteMultipleRegistersResponse(byte[] rawAdu)
            : base(rawAdu)
        { }
    }
}
