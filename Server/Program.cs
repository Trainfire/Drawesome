using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            server.Start();

            Console.WriteLine("Press Enter to stop the server...");
            Console.ReadLine();

            server.Stop();
        }
    }
}
