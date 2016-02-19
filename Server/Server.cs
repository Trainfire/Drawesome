using System;
using Fleck;

namespace Server
{
    public class Server
    {
        ConnectionsHandler ConnectionsHandler { get; set; }
        WebSocketServer SocketServer { get; set; }
        RoomManager RoomManager { get; set; }
        public Settings Settings { get; private set; }

        public Server()
        {
            var settings = new SettingsLoader();
            Settings = settings.Load();
        }

        public void Start()
        {
            SocketServer = new WebSocketServer(Settings.Server.HostUrl);
            ConnectionsHandler = new ConnectionsHandler(Settings);
            RoomManager = new RoomManager(ConnectionsHandler, Settings);

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
