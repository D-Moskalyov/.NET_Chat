using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Client");

            Socket client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Console.WriteLine("Enter server IP:");
            string ip_str = "127.0.0.1";

            int port = 2000;

            IPHostEntry ipList = Dns.Resolve(ip_str);
            IPAddress ip = ipList.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ip, port);

            client_socket.Connect(endPoint);

            byte[] firstAnswer = new byte[1024];
            int byteCount = client_socket.Receive(firstAnswer);
            string fromServerMessage = Encoding.UTF8.GetString(firstAnswer, 0, byteCount);
            Console.WriteLine(fromServerMessage);

            string message = string.Empty;
            message = Console.ReadLine();
            byte[] message_name = Encoding.UTF8.GetBytes(message);
            client_socket.Send(message_name);

            byte[] secondAnswer = new byte[1024];
            int byteCount2 = client_socket.Receive(secondAnswer);
            string fromServerMessage2 = Encoding.UTF8.GetString(secondAnswer, 0, byteCount);
            Console.WriteLine(fromServerMessage2);

            if (fromServerMessage2.Contains("Welcome"))
            {
                Thread ear = new Thread(EarMethod);
                ear.Start(client_socket);

                try
                {
                    while (true)
                    {
                        //Console.Write("Message: ");
                        message = Console.ReadLine();
                        byte[] mesage_buffer = Encoding.UTF8.GetBytes(message);
                        client_socket.Send(mesage_buffer);
                    }

                }
                catch (SocketException exp)
                {
                    Console.WriteLine("Error. " + exp.Message);
                }
            }

            //Console.Read();
        }

        static void EarMethod(object obj)
        {
            Socket ear_socket = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] messageByte = new byte[1024];
                    int byteCount = ear_socket.Receive(messageByte);
                    string message = Encoding.UTF8.GetString(messageByte, 0, byteCount);
                    Console.WriteLine(message);
                }
            }
            catch (SocketException exp)
            {
                Console.WriteLine("Error. " + exp.Message);
            }
        }
    }
}
