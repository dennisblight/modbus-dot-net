using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DennisBlight.Modbus
{
    public class ModbusListener
    {
        private Socket socket;

        public Socket Socket
        {
            get { return socket; }
        }

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
            get { return ((int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive)) > 0; }
            set { socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, value ? 1 : 0); }
        }

        public bool NoDelay
        {
            get { return ((int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay)) > 0; }
            set { socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, value ? 1 : 0); }
        }

        public bool ReuseAddress
        {
            get { return ((int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress)) > 0; }
            set { socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, value ? 1 : 0); }
        }

        public ModbusListener(IPAddress address, int port = 502)
        {
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(address, port));
        }

        public ModbusListener(string address, int port = 502)
            : this(Dns.GetHostAddresses(address).First(), port)
        { }

        public IPAddress RemoteIPAddress
        {
            get { return (socket.RemoteEndPoint as IPEndPoint).Address; }
        }

        public int RemotePort
        {
            get { return (socket.RemoteEndPoint as IPEndPoint).Port; }
        }

        public IPAddress LocalIPAddress
        {
            get { return (socket.LocalEndPoint as IPEndPoint).Address; }
        }

        public int LocalPort
        {
            get { return (socket.LocalEndPoint as IPEndPoint).Port; }
        }

        public void Start()
        {
            socket.Listen(5);
        }

        public void Stop()
        {
            socket.Close();
        }

        public ModbusTcpClient AcceptClient()
        {
            return new ModbusTcpClient(socket.Accept());
        }

        public async Task<ModbusTcpClient> AcceptClientAsync()
        {
            return new ModbusTcpClient(await socket.AcceptAsync());
        }
    }
}
