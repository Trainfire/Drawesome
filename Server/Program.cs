using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using Server;

namespace DrawesomeServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new WebSocketServer(8181);
            server.Log.Level = LogLevel.Trace;

            server.AddWebSocketService<Room>("/room");

            server.Start();

            Console.WriteLine("Press Enter to stop the server...");
            Console.ReadLine();

            server.Stop();
        }
    }
}
