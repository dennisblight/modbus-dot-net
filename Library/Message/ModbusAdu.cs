using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DennisBlight.Modbus.Message
{
    public class ModbusTcpAdu
    {
        private const byte TransactionIDOffset = 0;
        private const byte ProtocolIDOffset = 2;
        private const byte LengthOffset = 4;
        private const byte UnitIDOffset = 6;

        private byte[] adu;
        private byte[] mbap;
        private ModbusMessage message;

        public byte[] MBAPHeader
        {
            get
            {
                if (mbap == null)
                {
                    mbap = new byte[7];
                    Buffer.BlockCopy(adu, 0, mbap, 0, mbap.Length);
                }
                return mbap;
            }
        }

        public ModbusMessage Message => message;

        public ushort TransactionID
        {
            get { return BitHelper.ToUInt16(adu, TransactionIDOffset); }
            set { BitHelper.WriteBuffer(adu, value, TransactionID); }
        }

        public ushort ProtocolID => BitHelper.ToUInt16(adu, ProtocolIDOffset);

        public ushort Length => BitHelper.ToUInt16(adu, LengthOffset);

        public byte UnitID => adu[UnitIDOffset];

        public ModbusTcpAdu(ushort transactionID, byte unitID, ModbusMessage message)
        {
            this.message = message;
            adu = new byte[7 + message.Length];
            BitHelper.WriteBuffer(adu, transactionID, TransactionIDOffset);
            BitHelper.WriteBuffer(adu, message.Length, LengthOffset);
            adu[UnitIDOffset] = unitID;
            Buffer.BlockCopy(message.PDU, 0, adu, 7, message.Length);
        }

        public ModbusTcpAdu(ushort transactionID, ModbusMessage message)
            : this(transactionID, 0xff, message)
        { }

        public ModbusTcpAdu(ModbusMessage message)
            : this(0x0000, 0xff, message)
        { }

        public byte[] GetBytes()
        {
            return (byte[])adu.Clone();
        }

        /// <summary></summary>
        /// <param name="isRequest">Indentify wether modbus message is request type</param>
        public ModbusTcpAdu(byte[] buffer, bool isRequest)
        {
            adu = buffer;
            if (ProtocolID != 0) throw new IntegrityViolationException();
            if (Length != (buffer.Length - 6)) throw new IntegrityViolationException();
            byte[] pduSegment = new byte[buffer.Length - 7];
            Buffer.BlockCopy(buffer, 7, pduSegment, 0, pduSegment.Length);
            if(isRequest)
            {
                message = ModbusRequest.ParseBuffer(pduSegment);
            }
            else
            {
                message = ModbusResponse.ParseBuffer(pduSegment);
            }
        }
    }
}
