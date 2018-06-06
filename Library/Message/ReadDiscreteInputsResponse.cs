using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public class ReadDiscreteInputsResponse : Response
    {
        public byte ByteCount
        {
            get { return Adu[BaseSize + 0]; }
        }

        public bool[] GetInputStatus()
        {
            byte[] bytes = GetInputStatusBytes();
            bool[] inputs = new bool[bytes.Length * 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                for (int j = 0; j < 8; j++)
                {
                    inputs[8 * i + j] = ((b >> j) & 0x01) == 1;
                }
            }
            return inputs;
        }

        public byte[] GetInputStatusBytes()
        {
            byte[] bytes = new byte[ByteCount];
            Buffer.BlockCopy(Adu, BaseSize + 1, bytes, 0, bytes.Length);
            return bytes;
        }

        internal ReadDiscreteInputsResponse(byte[] rawAdu)
            : base(rawAdu)
        { }
    }
}
