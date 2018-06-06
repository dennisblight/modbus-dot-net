using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public class WriteSingleRegisterResponse : Response
    {
        public ushort RegisterAddress
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 0); }
        }

        public ushort RegisterValue
        {
            get { return BitHelper.ToUInt16(Adu, BaseSize + 2); }
        }

        internal WriteSingleRegisterResponse(byte[] rawAdu) 
            : base(rawAdu)
        { }
    }
}
