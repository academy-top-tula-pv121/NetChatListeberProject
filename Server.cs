using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace NetChatListenerProject
{
    internal class Server
    {
        static int port = 10001;
        static TcpListener serverListener;
        List<ClientInner> clients = new();

        public void AddConnect(ClientInner client)
        {
            clients.Add(client);
        }

        public void RemoveConnect(string id)
        {
            ClientInner client = clients.FirstOrDefault(c => c.id == id)!;
            if(client is not null)
                clients.Remove(client);
        }

        public void Listen()
        {
            try
            {
                serverListener = new TcpListener(IPAddress.Any, port);
                serverListener.Start();
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} Server is Listen...");

                while (true)
                {
                    TcpClient clientTcp = serverListener.AcceptTcpClient();
                    ClientInner client = new(clientTcp, this);
                    Thread clientThread = new Thread(client.Run);
                    clientThread.Start();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Stop();
            }
        }

        public void BroadcastSendMessage(string message, string id)
        {
            byte[] buffer = Encoding.Default.GetBytes(message);
            for(int i = 0; i < clients.Count; i++)
                if (clients[i].id != id)
                    clients[i].stream.Write(buffer, 0, buffer.Length);
        }

        public void Stop()
        {
            serverListener.Stop();
            for (int i = 0; i < clients.Count; i++)
                clients[i].Close();
        }
    }
}
