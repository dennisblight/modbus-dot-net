using DennisBlight.Modbus.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DennisBlight.Modbus
{
    public class ModbusClient : IDisposable
    {
        private Socket socket;

        public int SendBufferSize
        {
            get { return (int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer); }
            set { socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, value); }
        }

        public int ReceiveBufferSize
        {
            get { return (int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer); }
            set { socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, value); }
        }

        public int SendTimeout
        {
            get { return (int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout); }
            set { socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, value); }
        }

        public int ReceiveTimeout
        {
            get { return (int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout); }
            set { socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value); }
        }

        public bool KeepAlive
        {
            get { return (bool)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive); }
            set { socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, value); }
        }

        public bool NoDelay
        {
            get { return (bool)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay); }
            set { socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, value); }
        }

        public bool ReuseAddress
        {
            get { return (bool)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress); }
            set { socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, value); }
        }

        public IPAddress RemoteIPAddress
        {
            get { return (socket.RemoteEndPoint as IPEndPoint).Address; }
        }

        public int RemotePort
        {
            get { return (socket.RemoteEndPoint as IPEndPoint).Port; }
        }

        public void Dispose()
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Dispose();
            }
            socket = null;
        }

        ~ModbusClient()
        {
            Dispose();
        }

        public ModbusClient()
        {
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        public ModbusClient(string host, int port = 502)
            : this()
        {
            Connect(host, port);
        }

        public void Connect(string host, int port = 502)
        {
            socket.Connect(host, port);
        }
    }
}
