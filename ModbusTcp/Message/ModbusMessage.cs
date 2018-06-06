using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public abstract class ModbusMessage
    {
        //public const int TransactionIDIndex = 0;
        //public const int ProtocolIDIndex = 2;
        //public const int LengthIndex = 4;
        //public const int UnitIDIndex = 6;
        //public const int FunctionCodeIndex = 7;
        public const int BaseSize = 8;

        /// <summary>
        /// Application Data Unit
        /// </summary>
        protected byte[] Adu;

        /// <summary>
        /// Identification of a MODBUS Request / Response transaction.
        /// </summary>
        public int TransactionID
        {
            get { return BitHelper.ToInt16(Adu, 0); }
            set { BitHelper.WriteBuffer(Adu, (ushort)value, 0); }
        }

        /// <summary>
        /// 0 = MODBUS protocol
        /// </summary>
        public int ProtocolID { get { return 0; } }

        /// <summary>
        /// Identification of a remote slave connected on a serial line or on other buses.
        /// </summary>
        public int UnitID
        {
            get { return Adu[6]; }
            set { Adu[6] = (byte)value; }
        }

        /// <summary>
        /// Number of following bytes
        /// </summary>
        public int Length
        {
            get { return BitHelper.ToInt16(Adu, 4); }
            protected set { BitHelper.WriteBuffer(Adu, (ushort)value, 4); }
        }

        public FunctionCode FunctionCode
        {
            get { return (FunctionCode)(Adu[7] & 0x7f); }
        }

        public virtual byte[] GetBytes()
        {
            Length = Adu.Length - 6;
            return (byte[])Adu.Clone();
        }

        public byte[] GetMBAPHeader()
        {
            byte[] mbap = new byte[7];
            Buffer.BlockCopy(Adu, 0, mbap, 0, mbap.Length);
            return mbap;
        }

        protected ModbusMessage() { }

        protected ModbusMessage(int functionCode)
        {
            Adu = new byte[] { 0, 0, 0, 0, 0, 2, 0xFF, (byte)functionCode };
            TransactionID = (new Random()).Next(0xffff);
        }

        protected ModbusMessage(FunctionCode functionCode)
        {
            Adu = new byte[] { 0, 0, 0, 0, 0, 2, 0xFF, (byte)functionCode };
            TransactionID = (new Random()).Next(0xffff);
        }

        protected static void CheckConstraint(int value, int min, int max, string fieldName)
        {
            if (min <= value && value <= max) return;
            throw new ConstraintViolationException($"{fieldName} value must between {min} and {max}");
        }
    }
}
