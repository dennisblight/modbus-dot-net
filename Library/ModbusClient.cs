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
            KeepAlive = true;
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

        public void SendMessage(ModbusMessage message)
        {
            byte[] buffer = message.GetBytes();
            ArraySegment<byte> bufferSegment = new ArraySegment<byte>(buffer);
            socket.SendAsync(bufferSegment, SocketFlags.None).ContinueWith((i) => { });
        }

        public async Task SendMessageAsync(ModbusMessage message)
        {
            byte[] buffer = message.GetBytes();
            ArraySegment<byte> bufferSegment = new ArraySegment<byte>(buffer);
            await socket.SendAsync(bufferSegment, SocketFlags.None);
        }

        public Response ReceiveResponse()
        {
            byte[] buffer = new byte[300];
            int length = socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
            if (length != buffer.Length) Array.Resize(ref buffer, length);
            return Response.ParseBuffer(buffer);
        }

        public async Task<Response> ReceiveResponseAsync()
        {
            byte[] buffer = new byte[300];
            ArraySegment<byte> bufferSegment = new ArraySegment<byte>(buffer);
            int length = await socket.ReceiveAsync(bufferSegment, SocketFlags.None);
            if (length != buffer.Length) Array.Resize(ref buffer, length);
            return Response.ParseBuffer(buffer);
        }
    }
}
