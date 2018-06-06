using System;
using System.Collections.Generic;
using System.Text;

namespace DennisBlight.Modbus.Message
{
    public enum ExceptionCode : byte
    {
        /// <summary>
        /// The function code received in the query is not an allowable action for the server.
        /// </summary>
        IllegalFunction = 1,

        /// <summary>
        /// The data address received in the query is not an allowable address for the server.
        /// </summary>
        IllegalDataAddress = 2,

        /// <summary>
        /// A value contained in the query data field is not an allowable value for server.
        /// </summary>
        IllegalDataValue = 3,

        /// <summary>
        /// An unrecoverable error occurred while the server was attempting to perform the requested action.
        /// </summary>
        ServiceDeviceFailure = 4,

        /// <summary>
        /// Specialized use in conjunction with programming commands.
        /// The server has accepted the request and is processing it, but a long duration of time will be 
        /// required to do so.
        /// </summary>
        Acknowledge = 5,

        /// <summary>
        /// Specialized use in conjunction with programming commands.
        /// The server is engaged in processing a long–duration program command.
        /// </summary>
        ServerDeviceBusy = 6,

        /// <summary>
        /// Specialized use in conjunction with function codes 20 and 21 and reference type 6, to indicate 
        /// that the extended file area failed to pass a consistency check.
        /// </summary>
        MemoryParityError = 8,

        /// <summary>
        /// Specialized use in conjunction with gateways, indicates that the gateway was unable to allocate 
        /// an internal communication path from the input port to the output port for processing the request.
        /// </summary>
        GatewayPathUnavailable = 10,

        /// <summary>
        /// Specialized use in conjunction with gateways, indicates that no response was obtained from the 
        /// target device.
        /// </summary>
        GatewayTargetDeviceFailedToRespond = 11,

        NoException = 0
    }
}
