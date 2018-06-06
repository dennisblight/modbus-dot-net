using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public class ReadCoilsResponse : Response
    {
        public byte ByteCount
        {
            get { return Adu[BaseSize + 0]; }
        }

        public bool[] GetCoilStatus()
        {
            byte[] bytes = GetCoilStatusBytes();
            bool[] coils = new bool[bytes.Length * 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                for (int j = 0; j < 8; j++)
                {
                    coils[8 * i + j] = ((b >> j) & 0x01) == 1;
                }
            }
            return coils;
        }

        public byte[] GetCoilStatusBytes()
        {
            byte[] bytes = new byte[ByteCount];
            Buffer.BlockCopy(Adu, BaseSize + 1, bytes, 0, bytes.Length);
            return bytes;
        }

        internal ReadCoilsResponse(byte[] rawAdu)
            : base(rawAdu)
        { }
    }
}
