namespace NetChatListenerProject
{
    internal class Program
    {
        static Server server;
        static Thread serverThread;
        static void Main(string[] args)
        {
            try
            {
                server = new Server();
                serverThread = new(server.Listen);
                serverThread.Start();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                server.Stop();
            }
        }
    }
}