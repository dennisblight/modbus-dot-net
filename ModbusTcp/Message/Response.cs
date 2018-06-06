using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public class Response : ModbusMessage
    {
        public bool HasException
        {
            get { return (Adu[7] & 0x80) != 0; }
        }

        public ExceptionCode ExceptionCode
        {
            get { return (ExceptionCode)(HasException ? Adu[8] : 0); }
        }

        protected Response(byte[] rawAdu)
        {
            Adu = rawAdu;
        }

        public static Response ParseBuffer(byte[] buffer)
        {
            FunctionCode fc = (FunctionCode)(buffer[BaseSize - 1] & 0x80);
            switch(fc)
            {
                case FunctionCode.ReadCoils:
                    return new ReadCoilsResponse(buffer);
                case FunctionCode.ReadDiscreteInputs:
                    return new ReadDiscreteInputsResponse(buffer);
                case FunctionCode.ReadHoldingRegisters:
                    return new ReadHoldingRegistersResponse(buffer);
                case FunctionCode.ReadInputRegisters:
                    return new ReadInputRegistersResponse(buffer);
                case FunctionCode.WriteSingleCoil:
                    return new WriteSingleCoilResponse(buffer);
                case FunctionCode.WriteSingleRegister:
                    return new WriteSingleRegisterResponse(buffer);
                case FunctionCode.WriteMultipleCoils:
                    return new WriteMultipleCoilsResponse(buffer);
                case FunctionCode.WriteMultipleRegisters:
                    return new WriteMultipleRegistersResponse(buffer);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
