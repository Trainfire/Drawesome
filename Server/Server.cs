using Fleck;
using System;
using System.Collections.Generic;
using Shared;
using Newtonsoft.Json;
using System.Text;

namespace DrawesomeServer
{
    class Server : MessageEventHandler
    {
        List<IWebSocketConnection> Sockets = new List<IWebSocketConnection>();

        public void Start()
        {
            var server = new WebSocketServer("ws://127.0.0.1:8181");
            server.Start(socket =>
            {
                socket.OnOpen = () => OnOpen(socket);
                socket.OnClose = () => OnClose(socket);
                socket.OnMessage = (str) => OnMessage(str);
            });
        }

        void OnMessage(string message)
        {
            Console.WriteLine("Recieved Message: " + message);
            Sockets.ForEach(x => x.Send("Echo: " + message));
        }

        void OnOpen(IWebSocketConnection socket)
        {
            if (!Sockets.Contains(socket))
            {
                Console.WriteLine("Socket ID " + socket.ConnectionInfo.Id + " connected.");
                Sockets.Add(socket);
            }
        }

        void OnClose(IWebSocketConnection socket)
        {
            if (Sockets.Contains(socket))
            {
                Console.WriteLine("Socket ID " + socket.ConnectionInfo.Id + " disconnected");
                Sockets.Remove(socket);
            }
        }

        protected override void OnUserConnected(User user)
        {
            base.OnUserConnected(user);
            Console.WriteLine(user.Name + " connected to the game.");
        }

        protected override void OnUserSendMessage(UserMessage userMessage)
        {
            base.OnUserSendMessage(userMessage);
            Console.WriteLine(userMessage.User.Name + ": " + userMessage.Message);
        }

    }
}
