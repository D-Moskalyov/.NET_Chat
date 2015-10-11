using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Chat
{
    class Program
    {
        static List<User> users = new List<User>();

        static void Listen_Client(object user_socket)
        {
            //Console.WriteLine("Listen_Client start");

            Socket connection_socket = user_socket as Socket;
            if (connection_socket == null)
                return;

            byte[] messageForReg = Encoding.UTF8.GetBytes("Connected successfull \nEnter your name");
            connection_socket.Send(messageForReg);

            byte[] clientData = new byte[1024];

            int byteCount;

            try
            {
                byteCount = connection_socket.Receive(clientData);
            }
            catch (SocketException exp)
            {
                byteCount = 0;
                Console.WriteLine(exp.Message);
            }
            
            if (byteCount == 0)
                return;

            string clientName = Encoding.UTF8.GetString(clientData, 0, byteCount);

            bool newUser = true;
            foreach (User us in users)
            {
                if (us.name == clientName)
                {
                    newUser = false;
                    break;
                }
            }

            if (newUser)
            {
                Console.WriteLine(clientName + " connected");
                byte[] messageSecond = Encoding.UTF8.GetBytes("Welcome");
                connection_socket.Send(messageSecond);

                byte[] info = Encoding.UTF8.GetBytes(String.Concat("-*-*-*- ", clientName, " connected"));

                foreach (User us in users)
                {
                    us.socket.Send(info);
                }

                users.Add(new User(connection_socket, clientName));
            }
            else
            {
                byte[] messageSecond = Encoding.UTF8.GetBytes("Enter new name");
                connection_socket.Send(messageSecond);
                connection_socket.Shutdown(SocketShutdown.Both);
                connection_socket.Disconnect(false);
                return;
            }




            while (true)
            {
                try
                {
                    byte[] message_data = new byte[1024];

                    int byteCountMess = connection_socket.Receive(message_data);
                    if (byteCountMess == 0)
                        continue;

                    string clientMessage = Encoding.UTF8.GetString(message_data, 0, byteCountMess);

                    //if (clientMessage.IndexOf("close") != -1)
                    //break;

                    Console.WriteLine("Пользователь : {0}; Сообщение : {1}", clientName, clientMessage);

                    byte[] messageToClient = Encoding.UTF8.GetBytes(String.Format("Юзер : {0}; Сообщение : {1}", clientName, clientMessage));

                    foreach (User us in users)
                    {
                        if (us.name != clientName)
                            us.socket.Send(messageToClient);
                    }

                    //connection_socket.Send(messageToClient);
                }
                catch (SocketException exp)
                {
                    Console.WriteLine(clientName + " disconected");

                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].name == clientName)
                        {
                            users.RemoveAt(i);
                            break;
                        }
                    }

                    byte[] info = Encoding.UTF8.GetBytes(String.Concat("-*-*-*- ", clientName, " disconnected"));

                    foreach (User us in users)
                    {
                        us.socket.Send(info);
                    }

                    connection_socket.Shutdown(SocketShutdown.Both);
                    connection_socket.Disconnect(false);

                    return;

                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Start Server");

            Socket server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ipEndPoint = new System.Net.IPEndPoint(IPAddress.Any, 2000);

            server_socket.Bind(ipEndPoint);

            server_socket.Listen(5);

            while (true)
            {
                //Console.WriteLine("попытка установить подключение");
                Socket connection_socket = server_socket.Accept();

                if (connection_socket == null)
                {
                    continue;
                }

                //Console.WriteLine("Соединение произошло успешно");

                Thread client_thread = new Thread(Listen_Client);
                //Console.WriteLine("Начинаем Прослушивание");

                client_thread.Start(connection_socket);

            }
            //server_socket.Close();
            //Console.WriteLine("Server finish.");
        }

        static void NewUser()
        {

        }
        
    }
}
