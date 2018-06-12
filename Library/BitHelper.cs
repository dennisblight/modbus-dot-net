using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus
{
    public static class BitHelper
    {
        public static byte[] GetBytes(bool value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(char value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] GetBytes(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        public static void WriteBuffer(byte[] buffer, bool value, int index)
        {
            byte[] valueBinary = GetBytes(value);
            Buffer.BlockCopy(valueBinary, 0, buffer, index, valueBinary.Length);
        }
        public static void WriteBuffer(byte[] buffer, char value, int index)
        {
            byte[] valueBinary = GetBytes(value);
            Buffer.BlockCopy(valueBinary, 0, buffer, index, valueBinary.Length);
        }
        public static void WriteBuffer(byte[] buffer, short value, int index)
        {
            byte[] valueBinary = GetBytes(value);
            Buffer.BlockCopy(valueBinary, 0, buffer, index, valueBinary.Length);
        }
        public static void WriteBuffer(byte[] buffer, int value, int index)
        {
            byte[] valueBinary = GetBytes(value);
            Buffer.BlockCopy(valueBinary, 0, buffer, index, valueBinary.Length);
        }
        public static void WriteBuffer(byte[] buffer, long value, int index)
        {
            byte[] valueBinary = GetBytes(value);
            Buffer.BlockCopy(valueBinary, 0, buffer, index, valueBinary.Length);
        }
        public static void WriteBuffer(byte[] buffer, float value, int index)
        {
            byte[] valueBinary = GetBytes(value);
            Buffer.BlockCopy(valueBinary, 0, buffer, index, valueBinary.Length);
        }
        public static void WriteBuffer(byte[] buffer, double value, int index)
        {
            byte[] valueBinary = GetBytes(value);
            Buffer.BlockCopy(valueBinary, 0, buffer, index, valueBinary.Length);
        }
        public static void WriteBuffer(byte[] buffer, ushort value, int index)
        {
            byte[] valueBinary = GetBytes(value);
            Buffer.BlockCopy(valueBinary, 0, buffer, index, valueBinary.Length);
        }
        public static void WriteBuffer(byte[] buffer, uint value, int index)
        {
            byte[] valueBinary = GetBytes(value);
            Buffer.BlockCopy(valueBinary, 0, buffer, index, valueBinary.Length);
        }
        public static void WriteBuffer(byte[] buffer, ulong value, int index)
        {
            byte[] valueBinary = GetBytes(value);
            Buffer.BlockCopy(valueBinary, 0, buffer, index, valueBinary.Length);
        }

        public static Boolean ToBoolean(byte[] value, int index)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[sizeof(Boolean)];
                Buffer.BlockCopy(value, index, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToBoolean(bytes, 0);
            }
            return BitConverter.ToBoolean(value, index);
        }
        public static Char ToChar(byte[] value, int index)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[sizeof(Char)];
                Buffer.BlockCopy(value, index, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToChar(bytes, 0);
            }
            return BitConverter.ToChar(value, index);
        }
        public static Int16 ToInt16(byte[] value, int index)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[sizeof(Int16)];
                Buffer.BlockCopy(value, index, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToInt16(bytes, 0);
            }
            return BitConverter.ToInt16(value, index);
        }
        public static Int32 ToInt32(byte[] value, int index)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[sizeof(Int32)];
                Buffer.BlockCopy(value, index, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToInt32(bytes, 0);
            }
            return BitConverter.ToInt32(value, index);
        }
        public static Int64 ToInt64(byte[] value, int index)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[sizeof(Int64)];
                Buffer.BlockCopy(value, index, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToInt64(bytes, 0);
            }
            return BitConverter.ToInt64(value, index);
        }
        public static Single ToSingle(byte[] value, int index)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[sizeof(Single)];
                Buffer.BlockCopy(value, index, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToSingle(bytes, 0);
            }
            return BitConverter.ToSingle(value, index);
        }
        public static Double ToDouble(byte[] value, int index)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[sizeof(Double)];
                Buffer.BlockCopy(value, index, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToDouble(bytes, 0);
            }
            return BitConverter.ToDouble(value, index);
        }
        public static UInt16 ToUInt16(byte[] value, int index)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[sizeof(UInt16)];
                Buffer.BlockCopy(value, index, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToUInt16(bytes, 0);
            }
            return BitConverter.ToUInt16(value, index);
        }
        public static UInt32 ToUInt32(byte[] value, int index)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[sizeof(UInt32)];
                Buffer.BlockCopy(value, index, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToUInt32(bytes, 0);
            }
            return BitConverter.ToUInt32(value, index);
        }
        public static UInt64 ToUInt64(byte[] value, int index)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] bytes = new byte[sizeof(UInt64)];
                Buffer.BlockCopy(value, index, bytes, 0, bytes.Length);
                Array.Reverse(bytes);
                return BitConverter.ToUInt64(bytes, 0);
            }
            return BitConverter.ToUInt64(value, index);
        }

        public static long DoubleToInt64Bits(double value)
        {
            return BitConverter.DoubleToInt64Bits(value);
        }
        public static int SingleToInt32Bits(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return BitConverter.ToInt32(bytes, 0);
        }
        public static double Int64BitsToDouble(long value)
        {
            return BitConverter.Int64BitsToDouble(value);
        }
        public static float Int32BitsToSingle(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return BitConverter.ToSingle(bytes, 0);
        }
    }
}
