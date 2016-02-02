using System;
using System.Collections.Generic;
using Fleck;

namespace Server
{
    class Program
    {
        static List<WebSocketBehaviour> Handlers { get; set; }

        static void Main(string[] args)
        {
            var server = new WebSocketServer("ws://0.0.0.0:8181");

            AddWebSocketBehaviourHandler(new PlayerManager());

            server.Start(socket =>
            {
                socket.OnOpen += () => Handlers.ForEach(x => x.OnOpen(socket));
                socket.OnClose += () => Handlers.ForEach(x => x.OnClose(socket));
                socket.OnError += (ex) => Handlers.ForEach(x => x.OnError(ex));
                socket.OnMessage += (str) => Handlers.ForEach(x => x.OnMessage(str));
                socket.OnBinary += (bytes) => Handlers.ForEach(x => x.OnBinary(bytes));
            });

            Console.WriteLine("Press Enter to stop the server...");
            Console.ReadLine();

            server.Dispose();
        }

        static T AddWebSocketBehaviourHandler<T>(T instance) where T : WebSocketBehaviour
        {
            if (Handlers == null)
                Handlers = new List<WebSocketBehaviour>();

            if (Handlers.Contains(instance))
            {
                Console.WriteLine("Behaviours already contains {0}", instance.ToString());
            }
            else
            {
                Handlers.Add(instance);
            }

            return instance;
        }
    }
}
