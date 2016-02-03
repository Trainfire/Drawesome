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

        public List<Player> Players { get; private set; }
        public List<Room> Rooms { get; private set; }

        public ClientConnectionsHandler()
        {
            Rooms = new List<Room>();

            for (int i = 0; i < 10; i++)
            {
                Rooms.Add(new Room());
            }
        }

        public override void OnOpen(IWebSocketConnection socket)
        {
            base.OnOpen(socket);

            Logger.WriteLine("A connection was opened");

            // Respond to a join request by assigning a unique ID to the connection and sending it back to the client.
            var player = AddPlayer(socket);
            Send(new ServerMessage.ConnectionSuccess(player.ID), player);
        }

        public override void OnMessage(string m)
        {
            Logger.WriteLine("A message was recieved.");

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

            var message = JsonHelper.FromJson<Message>(m);
            if (message.Type == MessageType.ClientRequestRoomList)
            {
                var roomListMessage = JsonHelper.FromJson<ClientMessage.RequestRoomList>(m);
                Console.WriteLine("Recieved a request from {0} for a list a rooms.", roomListMessage.PlayerID);

                var target = Players.Find(x => x.ID == roomListMessage.PlayerID);

                var protocolRooms = Rooms.Select(x => x.Data).ToList();
                target.SendMessage(new ServerMessage.RoomList(protocolRooms));
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
