using System;
using Fleck;

namespace Server
{
    public class Server : ILogger
    {
        ConnectionsHandler ConnectionsHandler { get; set; }
        WebSocketServer SocketServer { get; set; }
        RoomManager RoomManager { get; set; }
        CommandHandler CommandHandler { get; set; }

        public Settings Settings { get; private set; }
        public bool Active { get; private set; }

        string ILogger.LogName { get { return "Server"; } }

        public Server()
        {
            LoadSettings();

            CommandHandler = new CommandHandler();
            CommandHandler.AddCommand(new CommandHandler.Command("quit", () => Stop()));
            CommandHandler.AddCommand(new CommandHandler.Command("reloadsettings", () => LoadSettings()));
        }

        public void Start()
        {
            Logger.Log(this, "Started");

            Active = true;

            SocketServer = new WebSocketServer(Settings.Server.HostUrl);
            ConnectionsHandler = new ConnectionsHandler(Settings);
            RoomManager = new RoomManager(ConnectionsHandler, Settings);

            SocketServer.Start(socket =>
            {
                socket.OnOpen += () => ConnectionsHandler.OnOpen(socket);
                socket.OnClose += () => ConnectionsHandler.OnClose(socket);
            });

            while(Active)
            {
                CommandHandler.ParseCommand(Console.ReadLine());
            }
        }

        public void Stop()
        {
            Logger.Log(this, "Stopped");
            SocketServer.Dispose();
            Active = false;
        }

        void LoadSettings()
        {
            Logger.Log(this, "Loading settings");
            var settings = new SettingsLoader().Load();

            if (settings != null)
                Settings = settings;
        }
    }
}
