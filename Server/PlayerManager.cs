using System;
using System.Linq;
using System.Collections.Generic;
using Fleck;
using Protocol;

namespace Server
{
    class PlayerManager : WebSocketBehaviour
    {
        public List<Player> Players { get; private set; }

        public override void OnOpen(IWebSocketConnection socket)
        {
            base.OnOpen(socket);

            // Assign a unique ID to the player and send it back to the client.
            var player = AddPlayer(socket);
            Send(new ValidatePlayer(player.ID), player);
        }

        public override void OnMessage(string m)
        {
            // Validates a player's request to change name. 
            // Usually sent on a first-time connection to the server after server-side validation.
            var playerConnectMessage = JsonHelper.FromJson<PlayerFirstConnectMessage>(m);
            Player matchingPlayer = null;
            if (playerConnectMessage != null)
            {
                foreach (var player in Players)
                {
                    if (player.ID == playerConnectMessage.ID)
                    {
                        player.Name = playerConnectMessage.PlayerName;
                        Logger.WriteLine("Player {0} connected.", player.Name);
                        matchingPlayer = player;
                        break;
                    }
                }

                // Send the latest player state to all clients.
                SendUpdateToAllClients();

                // Send Player Joined message
                SendToAll(new PlayerJoined(matchingPlayer), matchingPlayer);
            }
        }

        public override void OnClose(IWebSocketConnection socket)
        {
            base.OnClose(socket);

            var player = Players.Find(x => x.Socket == socket);
            if (player != null)
            {
                Players.Remove(player);
                Logger.WriteLine("Player {0} disconnected", player.Name);

                SendToAll(new PlayerLeft(player));
            }
        }

        Player AddPlayer(IWebSocketConnection socket)
        {
            if (Players == null)
                Players = new List<Player>();

            Player player = GetPlayerFromSocket(socket);

            if (player == null)
            {
                //Logger.WriteLine("A new player connected.");

                player = new Player("N/A", socket);
                player.ID = Guid.NewGuid().ToString();

                Players.Add(player);
            }
            else
            {
                Logger.WriteLine("Player {0} already exists...", player.Name);
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
            List<ProtocolPlayer> protocolPlayers = new List<ProtocolPlayer>();

            foreach (var player in Players)
            {
                var protocolPlayer = new ProtocolPlayer();
                protocolPlayer.ID = player.ID;
                protocolPlayer.Name = player.Name;
                protocolPlayers.Add(protocolPlayer);
            }

            var serverUpdate = new ServerUpdate(protocolPlayers);

            Players.ForEach(x => x.SendMessage(serverUpdate));
        }

        Player GetPlayerFromSocket(IWebSocketConnection socket)
        {
            return Players.FirstOrDefault(x => x.Socket == socket);
        }
    }
}
