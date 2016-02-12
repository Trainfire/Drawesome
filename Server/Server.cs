using System;
using Fleck;

namespace Server
{
    public class Server
    {
        ConnectionsHandler ConnectionsHandler { get; set; }
        WebSocketServer SocketServer { get; set; }
        Settings Settings { get; set; }

        public Server()
        {
            var settings = new SettingsLoader();
            Settings = settings.Load();
        }

        public void Start()
        {
            SocketServer = new WebSocketServer(Settings.ServerSettings.HostUrl);

            SocketServer.Start(socket =>
            {
                socket.OnOpen += () => ConnectionsHandler.OnOpen(socket);
                socket.OnClose += () => ConnectionsHandler.OnClose(socket);
                socket.OnMessage += (data) => ConnectionsHandler.OnMessage(data);
            });
        }

        public void Stop()
        {
            SocketServer.Dispose();
        }
    }
}
