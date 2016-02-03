using System;
using System.Linq;
using System.Collections.Generic;
using Fleck;
using Protocol;

namespace Server
{
    class ClientConnectionsHandler : WebSocketBehaviour
    {
        public event EventHandler<Player> PlayerConnected;
        public event EventHandler<Player> PlayerDisconnected;
        public event EventHandler<Player> PlayerCreateRoom;
        public event EventHandler<Player> PlayerJoinRoom;
        public event EventHandler<Player> PlayerLeaveRoom;

        public List<Player> Players { get; private set; }

        public override void OnOpen(IWebSocketConnection socket)
        {
            base.OnOpen(socket);

            // Respond to a join request by assigning a unique ID to the connection and sending it back to the client.
            var player = AddPlayer(socket);
            Send(new ServerMessage.ApproveClientConnection(player.ID), player);
        }

        public override void OnMessage(string m)
        {
            // Allow the connected client to assign their name.
            var clientConnectionRequest = JsonHelper.FromJson<ClientMessage.RequestConnection>(m);
            Player matchingPlayer = null;
            if (clientConnectionRequest != null)
            {
                foreach (var player in Players)
                {
                    if (player.ID == clientConnectionRequest.ID)
                    {
                        player.Name = clientConnectionRequest.PlayerName;
                        Logger.WriteLine("Player {0} connected.", player.Name);
                        matchingPlayer = player;
                        break;
                    }
                }

                // Send the latest player state to all clients.
                SendUpdateToAllClients();

                // Send Player Joined message.
                SendToAll(new ServerMessage.NotifyPlayerAction(PlayerAction.Connected));

                // Trigger event.
                if (PlayerConnected != null)
                    PlayerConnected(this, matchingPlayer);
            }
        }

        public override void OnClose(IWebSocketConnection socket)
        {
            base.OnClose(socket);

            // Remove disconnected player from manager.
            var player = Players.Find(x => x.Socket == socket);
            if (player != null)
            {
                Players.Remove(player);

                Logger.WriteLine("Player {0} disconnected", player.Name);

                SendToAll(new ServerMessage.NotifyPlayerAction(PlayerAction.Disconnected));

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
