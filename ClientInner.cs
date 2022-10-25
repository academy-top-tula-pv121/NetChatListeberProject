using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace NetChatListenerProject
{
    internal class ClientInner
    {
        protected internal string id { get; private set; }
        protected internal NetworkStream stream { get; private set; }

        protected string name;

        TcpClient client;

        Server server;

        public ClientInner(TcpClient client, Server server)
        {
            this.id = Guid.NewGuid().ToString();
            this.client = client;
            this.server = server;
            server.AddConnect(this);
        }

        public void Run()
        {
            try
            {
                stream = client.GetStream();
                string message = ReadMessage();
                name = message;

                message += " coming in chat";
                server.BroadcastSendMessage(message, this.id);
                Console.Write($">>> {DateTime.Now.ToShortTimeString()} ");
                Console.WriteLine($" > {message}");

                while(true)
                {
                    try
                    {
                        message = ReadMessage();

                        if (message.IndexOf("quit") != -1)
                            throw new Exception();

                        message = $"{name} > {message}";
                        server.BroadcastSendMessage(message, this.id);
                        Console.Write($">>> {DateTime.Now.ToShortTimeString()} ");
                        Console.WriteLine($" > {message}");
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"{name} leave chat");
                        server.BroadcastSendMessage($"{name} leave chat", this.id);
                        break;
                    }
                    
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Close();
            }
        }

        string ReadMessage()
        {
            byte[] buffer = new byte[1024];
            StringBuilder stringBuilder = new StringBuilder();
            int bufferSize = 0;

            do
            {
                bufferSize = stream.Read(buffer, 0, buffer.Length);
                stringBuilder.Append(Encoding.Default.GetString(buffer, 0, bufferSize));
            } while (stream.DataAvailable);

            return stringBuilder.ToString();
        }

        public void Close()
        {
            if (stream is not null)
                stream.Close();
            if (client is not null)
                client.Close();
        }
    }
}
