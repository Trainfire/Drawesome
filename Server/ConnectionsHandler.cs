using System;
using System.Linq;
using System.Collections.Generic;
using Fleck;
using Protocol;

namespace Server
{
    public class ConnectionsHandler
    {
        public event EventHandler<Player> PlayerConnected;
        public event EventHandler<Player> PlayerDisconnected;

        List<IConnectionMessageHandler> Handlers { get; set; }
        List<Player> Players { get; set; }
        Settings Settings { get; set; }

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
            Logger.WriteLine("A connection was opened");

            // Respond to a join request by assigning a unique ID to the connection and sending it back to the client.
            var player = AddPlayer(socket);
            Send(new ServerMessage.ConnectionSuccess(player.Data.ID), player);
        }

        public void OnMessage(string json)
        {
            Message.IsType<ClientMessage.RequestConnection>(json, (data) =>
            {
                Player matchingPlayer = null;
                foreach (var player in Players)
                {
                    if (player.Data.ID == data.ID)
                    {
                        player.Data.Name = data.PlayerName;

                        Logger.WriteLine("Player {0} connected.", player.Data.Name);

                        // Assign callback to recieve messages from the player
                        player.OnMessageString += OnPlayerMessage;

                        matchingPlayer = player;
                        break;
                    }
                }

                // Send the latest player state to all clients.
                SendUpdateToAllClients();

                // Send Player Joined message.
                SendToAll(new ServerMessage.NotifyPlayerAction(matchingPlayer.Data, PlayerAction.Connected));

                // Trigger event.
                if (PlayerConnected != null)
                    PlayerConnected(this, matchingPlayer);
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
            Handlers.ForEach(x => x.HandleMessage(player, json));
        }

        public void OnClose(IWebSocketConnection socket)
        {
            // Remove disconnected player from manager.
            var player = Players.Find(x => x.Socket == socket);
            if (player != null)
            {
                Players.Remove(player);

                Logger.WriteLine("Player {0} disconnected", player.Data.Name);

                SendToAll(new ServerMessage.NotifyPlayerAction(player.Data, PlayerAction.Disconnected));

                if (PlayerDisconnected != null)
                    PlayerDisconnected(this, player);
            }
        }

        Player AddPlayer(IWebSocketConnection socket)
        {
            if (Players == null)
                Players = new List<Player>();

            Player player = GetPlayerFromSocket(socket);

            if (player == null)
            {
                player = new Player("N/A", socket);
                player.Data.ID = Guid.NewGuid().ToString();

                Players.Add(player);
            }
            else
            {
                Logger.WriteLine("Player {0} already exists...", player.Data.Name);
            }

            return player;
        }

        public void Send(Message message, Player player)
        {
            player.SendMessage(message);
        }

        public void SendToAll(Message message)
        {
            Players.ForEach(x => x.SendMessage(message));
        }

        public void SendToAll(Message message, Player exception)
        {
            var players = Players.Where(x => x != exception).ToList();
            players.ForEach(x => x.SendMessage(message));
        }

        /// <summary>
        /// Sends a Server Update to all Clients.
        /// </summary>
        public void SendUpdateToAllClients()
        {
            List<PlayerData> protocolPlayers = Players.Select(x => x.Data).ToList();

            var serverUpdate = new ServerUpdate(protocolPlayers);

            Players.ForEach(x => x.SendMessage(serverUpdate));
        }

        Player GetPlayerFromSocket(IWebSocketConnection socket)
        {
            return Players.FirstOrDefault(x => x.Socket == socket);
        }
    }
}
