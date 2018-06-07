using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DennisBlight.Modbus;
using DennisBlight.Modbus.Message;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ModbusClient client = new ModbusClient();
            Console.WriteLine($"Keep Alive          : {client.KeepAlive}");
            Console.WriteLine($"Receive Buffer Size : {client.ReceiveBufferSize = 1024}");
            Console.WriteLine($"Send Buffer Size    : {client.SendBufferSize = 1024}");
            Console.WriteLine($"Receive Timeout     : {client.ReceiveTimeout = 500}");
            Console.WriteLine($"Send Timeout        : {client.SendTimeout = 500}");
            Console.WriteLine($"No Delay            : {client.NoDelay}");
            Console.WriteLine($"Reuse Address       : {client.ReuseAddress = true}");
            client.Connect("localhost");
            bool running = true;
            Task.Run(() =>
            {
                try
                {
                    while (running && client.Connected)
                    {
                        int length = client.SendMessage(new WriteSingleRegisterRequest(9999, 1000));
                        Console.WriteLine($"{length} bytes sent");
                        Thread.Sleep(200);
                    }
                    if (client.Connected)
                    {
                        Console.WriteLine("Stopped");
                    }
                    else
                    {
                        Console.WriteLine("Disconnected");

                    }
                }
                catch (Exception x)
                {
                    Console.WriteLine($"Error: {x.Message}");
                    client = new ModbusClient("localhost");
                }
            });
            Console.ReadLine();
            running = false;
            TcpListener listener = new TcpListener(IPAddress.Loopback, 502);
            //new ArraySegment<byte>()
        }
    }
}
