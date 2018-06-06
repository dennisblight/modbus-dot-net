using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus
{
    public class ConstraintViolationException : Exception
    {
        public ConstraintViolationException(string message) : base(message) { }
    }
}
