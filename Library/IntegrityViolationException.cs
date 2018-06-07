using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DennisBlight.Modbus
{
    public class IntegrityViolationException : Exception
    {
        public IntegrityViolationException()
            : base("Raw binaries data doesn't pass some integrity rule set.")
        { }
    }
}
