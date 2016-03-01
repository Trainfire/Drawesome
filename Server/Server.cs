using System;
using Fleck;
using Newtonsoft.Json;

namespace Server
{
    public class Server : ILogger
    {
        ConnectionsHandler ConnectionsHandler { get; set; }
        WebSocketServer SocketServer { get; set; }
        RoomManager RoomManager { get; set; }
        CommandHandler CommandHandler { get; set; }

        SettingsLoader SettingsLoader { get; set; }
        bool Active { get; set; }

        string ILogger.LogName { get { return "Server"; } }

        public Server()
        {
            SettingsLoader = new SettingsLoader();
            LoadSettings();

            CommandHandler = new CommandHandler();
            CommandHandler.AddCommand(new CommandHandler.Command("quit", () => Stop()));
            CommandHandler.AddCommand(new CommandHandler.Command("reloadsettings", () => LoadSettings()));
        }

        public void Start()
        {
            Logger.Log(this, "Started");

            Active = true;

            SocketServer = new WebSocketServer(SettingsLoader.Values.Server.HostUrl);
            ConnectionsHandler = new ConnectionsHandler(SettingsLoader);
            RoomManager = new RoomManager(ConnectionsHandler, SettingsLoader);

            SocketServer.Start(socket =>
            {
                socket.OnOpen += () => ConnectionsHandler.OnOpen(socket);
                socket.OnClose += () => ConnectionsHandler.OnClose(socket);
            });

            while (Active)
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
            SettingsLoader.Load();
        }
    }
}
