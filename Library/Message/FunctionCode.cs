using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public enum FunctionCode : byte
    {
        /// <summary>
        /// This function code is used to read status of coils in a remote device.
        /// </summary>
        ReadCoils = 1,

        /// <summary>
        /// This function code is used to read status of discrete inputs in a remote device.
        /// </summary>
        ReadDiscreteInputs = 2,

        /// <summary>
        /// This function code is used to read the contents of a contiguous block of holding registers in a remote device.
        /// </summary>
        ReadHoldingRegisters = 3,

        /// <summary>
        /// This function code is used to read a block of contiguous input registers in a remote device.
        /// </summary>
        ReadInputRegisters = 4,

        /// <summary>
        /// This function code is used to write a single output to either ON or OFF in a remote device.
        /// </summary>
        WriteSingleCoil = 5,

        /// <summary>
        /// This function code is used to write a single holding register in a remote device.
        /// </summary>
        WriteSingleRegister = 6,

        /// <summary>
        /// This function code is used to force each coil in a sequence of coils to either ON or OFF in a remote device.
        /// </summary>
        WriteMultipleCoils = 15,

        /// <summary>
        /// This function code is used to write a block of contiguous registers in a remote device.
        /// </summary>
        WriteMultipleRegisters = 16
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed public class FunctionCodeAttribute : Attribute
    {
        private readonly FunctionCode functionCode;

        public FunctionCodeAttribute(FunctionCode functionCode)
        {
            this.functionCode = functionCode;
        }

        public FunctionCode FunctionCode => functionCode;
    }
}
