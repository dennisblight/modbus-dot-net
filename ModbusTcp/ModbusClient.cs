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
        private TcpClient client;
        
        public int ReceiveBufferSize
        {
            get { return client.ReceiveBufferSize; }
            set { client.ReceiveBufferSize = value; }
        }

        public int SendBufferSize
        {
            get { return client.SendBufferSize; }
            set { client.SendBufferSize = value; }
        }

        public int ReceiveTimeout
        {
            get { return client.ReceiveTimeout; }
            set { client.ReceiveTimeout = value; }
        }

        public int SendTimeout
        {
            get { return client.SendTimeout; }
            set { client.SendTimeout = value; }
        }

        public bool Connected
        {
            get { return client.Connected; }
        }

        public ModbusClient()
        {
            //client = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        public ModbusClient(string hostname, int port = 502)
        {
            client = new TcpClient(hostname, port);
        }

        public void Connect(string hostname, int port = 502)
        {
            client.Connect(hostname, port);
        }

        public void Connect(IPAddress ipAddress, int port = 502)
        {
            client.Connect(ipAddress, port);
        }

        public async Task ConnectAsync(string hostname, int port = 502)
        {
            await client.ConnectAsync(hostname, port);
        }

        public async Task ConnectAsync(IPAddress ipAddress, int port = 502)
        {
            await client.ConnectAsync(ipAddress, port);
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public void SendRequest(ModbusMessage message)
        {
            byte[] buffer = message.GetBytes();
            client.GetStream().Write(buffer, 0, buffer.Length);
        }

        public async Task SendRequestAsync(ModbusMessage message)
        {
            await Task.Run(() => SendRequest(message));
        }

        public Response ReadResponse()
        {
            byte[] buffer = new byte[client.Available];
            int readedCount = client.GetStream().Read(buffer, 0, buffer.Length);
            if (buffer.Length != readedCount) Array.Resize(ref buffer, readedCount);
            return Response.ParseBuffer(buffer);
        }

        public async Task<Response> ReadResponseAsync()
        {
            return await Task.Run(() => ReadResponse());
        }
    }
}
