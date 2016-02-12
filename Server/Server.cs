using System;
using Fleck;

namespace Server
{
    public class Server
    {
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

            });
        }

        public void Stop()
        {
            SocketServer.Dispose();
        }
    }
}
