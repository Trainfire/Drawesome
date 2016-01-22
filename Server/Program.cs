using System;
using Fleck;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new WebSocketServer("ws://0.0.0.0:8181");
            var room = new PlayerManager();
            server.Start(socket =>
            {
                socket.OnOpen += () => room.OnOpen(socket);
                socket.OnClose += () => room.OnClose(socket);
                socket.OnError += (ex) => room.OnError(ex);
                socket.OnMessage += (str) => room.OnMessage(str);
            });

            Console.WriteLine("Press Enter to stop the server...");
            Console.ReadLine();

            server.Dispose();
        }
    }
}
