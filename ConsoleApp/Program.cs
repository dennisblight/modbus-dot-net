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
using System.Reflection;
using static System.Diagnostics.Contracts.Contract;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RawTcp();return;//
            byte[] buffer = new byte[] { 0x01, 0x00, 0x13, 0x00, 0x13 };
            ModbusMessage mm = ModbusRequest.ParseBuffer(buffer);
            return;
            //ReflectMyClasses();return;
            //TestMessage();return;
            ModbusTcpClient client = new ModbusTcpClient();
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
                    client = new ModbusTcpClient("localhost");
                }
            });

            Console.ReadLine();
            running = false;
            //new ArraySegment<byte>()
        }

        static void Buffering()
        {
            object[] dataModels = new object[] {12, 12L, 32ul, 4.0f, 5.4d  };
            Console.WriteLine(Buffer.ByteLength(dataModels));
        }

        static void TestMessage()
        {
            ModbusMessage fc01Req = new ReadCoilsRequest(0x13, 0x13);
            ModbusMessage fc01Res = new ReadCoilsResponse(true, false, true, true, false, false, true, true, true, true, false, true, false, true, true, false, true, false, true, false);
            ModbusMessage fc02Req = new ReadDiscreteInputsRequest(0xc4, 0x16);
            ModbusMessage fc02Res = new ReadDiscreteInputsResponse(false, false, true, true, false, true, false, true, true, true, false, true, true, false, true, true, true, false, true, false, true, true);
            ModbusMessage fc03Req = new ReadHoldingRegistersRequest(0x6b, 0x03);
            ModbusMessage fc03Res = new ReadHoldingRegistersResponse(0x022b, 0x0000, 0x0064);
            ModbusMessage fc04Req = new ReadInputRegistersRequest(0x08, 0x01);
            ModbusMessage fc04Res = new ReadInputRegistersResponse(0x000a);
            ModbusMessage fc05Req = new WriteSingleCoilRequest(0xac, true);
            ModbusMessage fc05Res = new WriteSingleCoilResponse(0xac, true);
            ModbusMessage fc06Req = new WriteSingleRegisterRequest(0x01, 0x0003);
            ModbusMessage fc06Res = new WriteSingleRegisterResponse(0x01, 0x0003);
            ModbusMessage fc0fReq = new WriteMultipleCoilsRequest(0x13, true, false, true, true, false, false, true, true, true, false);
            ModbusMessage fc0fRes = new WriteMultipleCoilsResponse(0x13, 0x0a);
            ModbusMessage fc10Req = new WriteMultipleRegistersRequest(0x01, 0x000a, 0x0102);
            ModbusMessage fc10Res = new WriteMultipleRegistersResponse(0x01, 0x02);

            PrintMessagePDU(fc01Req);
            PrintMessagePDU(fc01Res);
            PrintMessagePDU(fc02Req);
            PrintMessagePDU(fc02Res);
            PrintMessagePDU(fc03Req);
            PrintMessagePDU(fc03Res);
            PrintMessagePDU(fc04Req);
            PrintMessagePDU(fc04Res);
            PrintMessagePDU(fc05Req);
            PrintMessagePDU(fc05Res);
            PrintMessagePDU(fc06Req);
            PrintMessagePDU(fc06Res);
            PrintMessagePDU(fc0fReq);
            PrintMessagePDU(fc0fRes);
            PrintMessagePDU(fc10Req);
            PrintMessagePDU(fc10Res);

            Assert(MessagePDU(fc01Req) == "Req.: 01 00 13 00 13", $"{nameof(fc01Req)} Fail");
            Assert(MessagePDU(fc01Res) == "Res.: 01 03 cd 6b 05", $"{nameof(fc01Res)} Fail");
            Assert(MessagePDU(fc02Req) == "Req.: 02 00 c4 00 16", $"{nameof(fc02Req)} Fail");
            Assert(MessagePDU(fc02Res) == "Res.: 02 03 ac db 35", $"{nameof(fc02Res)} Fail");
            Assert(MessagePDU(fc03Req) == "Req.: 03 00 6b 00 03", $"{nameof(fc03Req)} Fail");
            Assert(MessagePDU(fc03Res) == "Res.: 03 06 02 2b 00 00 00 64", $"{nameof(fc03Res)} Fail");
            Assert(MessagePDU(fc04Req) == "Req.: 04 00 08 00 01", $"{nameof(fc04Req)} Fail");
            Assert(MessagePDU(fc04Res) == "Res.: 04 02 00 0a", $"{nameof(fc04Res)} Fail");
            Assert(MessagePDU(fc05Req) == "Req.: 05 00 ac ff 00", $"{nameof(fc05Req)} Fail");
            Assert(MessagePDU(fc05Res) == "Res.: 05 00 ac ff 00", $"{nameof(fc05Res)} Fail");
            Assert(MessagePDU(fc06Req) == "Req.: 06 00 01 00 03", $"{nameof(fc06Req)} Fail");
            Assert(MessagePDU(fc06Res) == "Res.: 06 00 01 00 03", $"{nameof(fc06Res)} Fail");
            Assert(MessagePDU(fc0fReq) == "Req.: 0f 00 13 00 0a 02 cd 01", $"{nameof(fc0fReq)} Fail");
            Assert(MessagePDU(fc0fRes) == "Res.: 0f 00 13 00 0a", $"{nameof(fc0fRes)} Fail");
            Assert(MessagePDU(fc10Req) == "Req.: 10 00 01 00 02 04 00 0a 01 02", $"{nameof(fc10Req)} Fail");
            Assert(MessagePDU(fc10Res) == "Res.: 10 00 01 00 02", $"{nameof(fc10Res)} Fail");

            Assert(fc01Req.FunctionCode == (FunctionCode)0x01);
            Assert(fc01Res.FunctionCode == (FunctionCode)0x01);
            Assert(fc02Req.FunctionCode == (FunctionCode)0x02);
            Assert(fc02Res.FunctionCode == (FunctionCode)0x02);
            Assert(fc03Req.FunctionCode == (FunctionCode)0x03);
            Assert(fc03Res.FunctionCode == (FunctionCode)0x03);
            Assert(fc04Req.FunctionCode == (FunctionCode)0x04);
            Assert(fc04Res.FunctionCode == (FunctionCode)0x04);
            Assert(fc05Req.FunctionCode == (FunctionCode)0x05);
            Assert(fc05Res.FunctionCode == (FunctionCode)0x05);
            Assert(fc06Req.FunctionCode == (FunctionCode)0x06);
            Assert(fc06Res.FunctionCode == (FunctionCode)0x06);
            Assert(fc0fReq.FunctionCode == (FunctionCode)0x0f);
            Assert(fc0fRes.FunctionCode == (FunctionCode)0x0f);
            Assert(fc10Req.FunctionCode == (FunctionCode)0x10);
            Assert(fc10Res.FunctionCode == (FunctionCode)0x10);
        }

        static void PrintMessagePDU(ModbusMessage message)
        {
            Console.WriteLine(MessagePDU(message));
        }

        static string MessagePDU(ModbusMessage message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{(message is ModbusRequest ? "Req." : "Res.")}: ");
            foreach (var b in message.GetBytes()) sb.Append($"{b:x2} ");
            return sb.ToString().Trim();
        }

        static void ReflectMyClasses()
        {
            var types = Assembly.GetAssembly(typeof(ModbusMessage)).GetTypes().Where((t)=> t.IsSubclassOf(typeof(ModbusMessage)) && t.GetCustomAttribute(typeof(FunctionCodeAttribute)) != null);
            Console.WriteLine(types.Count());
            foreach(var t in types)
            {
                Console.WriteLine(t.Name);
            }
        }

        static void RawTcp()
        {
            var client = new TcpClient("192.168.44.3", 502);
            var stream = client.GetStream();
            while(true)
            {
                byte[] write = Encoding.ASCII.GetBytes("test\r\n");
                stream.Write(write, 0, write.Length);
                byte[] read = new byte[300];
                int length = stream.Read(read, 0, read.Length);
                var dist = BitConverter.ToUInt16(read, 0) * 0.034 / 2;
                Console.WriteLine(dist);
                Thread.Sleep(500);
            }
        }
    }
}
