using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public class WriteSingleCoilResponse : Response
    {
        public ushort OutputAddress
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 0); }
        }

        public bool OutputValue
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 2) > 0; }
        }

        internal WriteSingleCoilResponse(byte[] rawAdu) 
            : base(rawAdu)
        { }
    }
}
