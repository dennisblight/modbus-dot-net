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
    public class ModbusTcpClient : IDisposable
    {
        private Socket socket;

        public Socket Socket => socket;

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

        public bool Connected => socket.Connected;

        public IPAddress RemoteIPAddress => (socket.RemoteEndPoint as IPEndPoint).Address;

        public int RemotePort => (socket.RemoteEndPoint as IPEndPoint).Port;

        public IPAddress LocalIPAddress => (socket.LocalEndPoint as IPEndPoint).Address;

        public int LocalPort => (socket.LocalEndPoint as IPEndPoint).Port;

        public void Dispose()
        {
            if (socket != null)
            {
                if (socket.Connected) socket.Shutdown(SocketShutdown.Both);
                socket.Dispose();
            }
            socket = null;
        }

        ~ModbusTcpClient()
        {
            Dispose();
        }

        public ModbusTcpClient()
        {
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            KeepAlive = true;
            ReceiveBufferSize = 1024;
            SendBufferSize = 1024;
        }

        public ModbusTcpClient(string host, int port = 502)
            : this()
        {
            Connect(host, port);
        }

        internal ModbusTcpClient(Socket socket)
        {
            this.socket = socket;
        }

        public void Connect(string host, int port = 502)
        {
            socket.Connect(host, port);
        }

        public void Connect(IPAddress host, int port = 502)
        {
            socket.Connect(host, port);
        }

        public async Task ConnectAsync(string host, int port = 502)
        {
            await socket.ConnectAsync(host, port);
        }

        public async Task ConnectAsync(IPAddress host, int port = 502)
        {
            await socket.ConnectAsync(host, port);
        }

        public int SendMessage(ModbusTcpAdu message)
        {
            return socket.Send(message.GetBytes());
        }

        public int SendMessage(ushort transactionID, byte unitID, ModbusMessage message)
        {
            return SendMessage(new ModbusTcpAdu(transactionID, unitID, message));
        }

        public int SendMessage(ushort transactionID, ModbusMessage message)
        {
            return SendMessage(transactionID, 0xff, message);
        }

        public int SendMessage(ModbusMessage message)
        {
            return SendMessage(0x0000, 0xff, message);
        }

        public async Task<int> SendMessageAsync(ModbusTcpAdu message)
        {
            return await socket.SendAsync(new ArraySegment<byte>(message.GetBytes()), SocketFlags.None);
        }

        public async Task<int> SendMessageAsync(ushort transactionID, byte unitID, ModbusMessage message)
        {
            return await SendMessageAsync(new ModbusTcpAdu(transactionID, unitID, message));
        }

        public async Task<int> SendMessageAsync(ushort transactionID, ModbusMessage message)
        {
            return await SendMessageAsync(transactionID, 0xff, message);
        }

        public async Task<int> SendMessageAsync(ModbusMessage message)
        {
            return await SendMessageAsync(0x0000, 0xff, message);
        }

        public ModbusTcpAdu ReceiveResponse(bool isRequest)
        {
            byte[] buffer = new byte[300];
            int length = socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
            if (length != buffer.Length) Array.Resize(ref buffer, length);
            return new ModbusTcpAdu(buffer, isRequest);
        }

        public ModbusTcpAdu ReceiveResponse()
        {
            return ReceiveResponse(false);
        }

        public async Task<ModbusTcpAdu> ReceiveResponseAsync(bool isRequest)
        {
            byte[] buffer = new byte[300];
            int length = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            if (length != buffer.Length) Array.Resize(ref buffer, length);
            return new ModbusTcpAdu(buffer, isRequest);
        }

        public async Task<ModbusTcpAdu> ReceiveResponseAsync()
        {
            return await ReceiveResponseAsync(false);
        }
    }
}
