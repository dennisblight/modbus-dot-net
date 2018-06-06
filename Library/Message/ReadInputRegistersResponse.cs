using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public class ReadInputRegistersResponse : Response
    {
        public byte ByteCount
        {
            get { return Adu[BaseSize + 0]; }
        }

        public ushort[] GetRegisterValue()
        {
            byte[] bytes = GetRegisterValueBytes();
            ushort[] registers = new ushort[bytes.Length / 2];
            for (int i = 0; i < registers.Length; i++)
            {
                registers[i] = BitHelper.ToUInt16(bytes, 2 * i);
            }
            return registers;
        }

        public byte[] GetRegisterValueBytes()
        {
            byte[] bytes = new byte[ByteCount];
            Buffer.BlockCopy(Adu, BaseSize + 1, bytes, 0, bytes.Length);
            return bytes;
        }

        internal ReadInputRegistersResponse(byte[] rawAdu)
            : base(rawAdu)
        { }
    }
}
