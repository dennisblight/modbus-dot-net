using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public class WriteMultipleCoilsResponse : Response
    {
        public ushort StartingAddress
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 0); }
        }

        public ushort Quantity
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 2); }
        }

        internal WriteMultipleCoilsResponse(byte[] rawAdu)
            : base(rawAdu)
        { }
    }
}
