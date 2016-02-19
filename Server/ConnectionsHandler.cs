using System;
using System.Linq;
using System.Collections.Generic;
using Fleck;
using Protocol;

namespace Server
{
    /// <summary>
    /// This class handles clients connecting and disconnecting, as well as messages.
    /// Messages are relayed to any assigned handlers that implement IConnectionMessageHandler.
    /// </summary>
    public class ConnectionsHandler : ILogger
    {
        public event EventHandler<Player> PlayerConnected;
        public event EventHandler<Player> PlayerDisconnected;

        public string LogName { get { return "Connections Handler"; } }

        List<IConnectionMessageHandler> Handlers { get; set; }
        List<Player> ConnectedPlayers { get; set; }
        Settings Settings { get; set; }

        static readonly object _lock = new object();

        public ConnectionsHandler(Settings settings)
        {
            Settings = settings;
            Handlers = new List<IConnectionMessageHandler>();
        }

        public void AddMessageListener(IConnectionMessageHandler handler)
        {
            if (!Handlers.Contains(handler))
                Handlers.Add(handler);
        }

        public void OnOpen(IWebSocketConnection socket)
        {
            Logger.Log(this, "A connection was opened");

            // Respond to a join request by assigning a unique ID to the connection and sending it back to the client.
            var player = AddPlayer(socket);
            Send(new ServerMessage.ConnectionSuccess(player.Data), player);
        }

        public void OnMessage(string json)
        {
            Message.IsType<ClientMessage.RequestConnection>(json, (data) =>
            {
                var matchingConnections = ConnectedPlayers.Where(x => x.Data.ID == data.PlayerInfo.ID).ToList();

                if (matchingConnections.Count > 1)
                {
                    Logger.Warn(this, "There is more than one player on the server with ID '{0}'. This is very bad!", data.PlayerInfo.ID);
                    return;
                }

                var matchingPlayer = matchingConnections.First();

                if (matchingPlayer == null)
                {
                    Logger.Warn(this, "Could not find connection matching the ID '{0}'", data.PlayerInfo.ID);
                }
                else
                {
                    // Assign the requested name
                    matchingPlayer.Data.SetName(data.Name);

                    Logger.Log(this, "Player {0} connected.", matchingPlayer.Data.Name);

                    // Send the latest player state to all clients.
                    SendUpdateToAllClients();

                    // Send Player Joined message.
                    SendToAll(new ServerMessage.NotifyPlayerAction(matchingPlayer.Data, PlayerAction.Connected));

                    // Trigger event.
                    if (PlayerConnected != null)
                        PlayerConnected(this, matchingPlayer);

                    // Assign callback
                    matchingPlayer.OnMessageString += OnPlayerMessage;
                }
            });
        }

        /// <summary>
        /// Passes a player's message to any interested listeners.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="json"></param>
        void OnPlayerMessage(object sender, string json)
        {
            var player = sender as Player;
            Handlers.ToList().ForEach(x => x.HandleMessage(player, json));
        }

        public void OnClose(IWebSocketConnection socket)
        {
            // Remove disconnected player from manager.
            var player = ConnectedPlayers.Find(x => x.Socket == socket);
            if (player != null)
            {
                ConnectedPlayers.Remove(player);

                Logger.Log(this, "Player {0} disconnected", player.Data.Name);

                SendToAll(new ServerMessage.NotifyPlayerAction(player.Data, PlayerAction.Disconnected));

                if (PlayerDisconnected != null)
                    PlayerDisconnected(this, player);
            }
        }

        Player AddPlayer(IWebSocketConnection socket)
        {
            if (ConnectedPlayers == null)
                ConnectedPlayers = new List<Player>();

            Player player = GetPlayerFromSocket(socket);

            if (player == null)
            {
                player = new Player("N/A", socket);
                player.Data.ID = Guid.NewGuid().ToString();

                ConnectedPlayers.Add(player);
            }
            else
            {
                Logger.Warn(this, "Player {0} already exists...", player.Data.Name);
            }

            return player;
        }

        public void Send(Message message, Player player)
        {
            player.SendMessage(message);
        }

        public void SendToAll(Message message)
        {
            ConnectedPlayers.ForEach(x => x.SendMessage(message));
        }

        public void SendToAll(Message message, Player exception)
        {
            var players = ConnectedPlayers.Where(x => x != exception).ToList();
            players.ForEach(x => x.SendMessage(message));
        }

        /// <summary>
        /// Sends a Server Update to all Clients.
        /// </summary>
        public void SendUpdateToAllClients()
        {
            List<PlayerData> protocolPlayers = ConnectedPlayers.Select(x => x.Data).ToList();

            var serverUpdate = new ServerUpdate(protocolPlayers);

            ConnectedPlayers.ForEach(x => x.SendMessage(serverUpdate));
        }

        Player GetPlayerFromSocket(IWebSocketConnection socket)
        {
            return ConnectedPlayers.FirstOrDefault(x => x.Socket == socket);
        }
    }
}
