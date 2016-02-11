using System;
using System.Linq;
using System.Collections.Generic;
using Fleck;
using Protocol;

namespace Server
{
    public class ClientConnectionsHandler : WebSocketBehaviour
    {
        public event EventHandler<Player> PlayerConnected;
        public event EventHandler<Player> PlayerDisconnected;

        public List<Player> Players { get; private set; }
        public List<Room> Rooms { get; private set; }

        public ClientConnectionsHandler()
        {
            Rooms = new List<Room>();
        }

        public override void OnOpen(IWebSocketConnection socket)
        {
            base.OnOpen(socket);

            Logger.WriteLine("A connection was opened");

            // Respond to a join request by assigning a unique ID to the connection and sending it back to the client.
            var player = AddPlayer(socket);
            Send(new ServerMessage.ConnectionSuccess(player.Data.ID), player);
        }

        public override void OnMessage(string json)
        {
            Message.IsType<ClientMessage.RequestConnection>(json, (data) =>
            {
                Player matchingPlayer = null;
                foreach (var player in Players)
                {
                    if (player.Data.ID == data.ID)
                    {
                        player.Data.Name = data.Name;
                        Logger.WriteLine("Player {0} connected.", player.Data.Name);
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

            Message.IsType<ClientMessage.RequestRoomList>(json, (data) =>
            {
                Console.WriteLine("Recieved a request from {0} for a list a rooms.", data.Player.ID);

                var target = Players.Find(x => x.Data.ID == data.Player.ID);

                var protocolRooms = Rooms.Select(x => x.Data).ToList();
                target.SendMessage(new ServerMessage.RoomList(protocolRooms));
            });

            Message.IsType<ClientMessage.JoinRoom>(json, (data) =>
            {
                Console.WriteLine("Recieved a request from {0} to join room {1}.", data.Player.ID, data.RoomId);

                var roomHasPlayer = Rooms.Find(x => x.HasPlayer(data.Player));
                if (roomHasPlayer != null)
                    roomHasPlayer.Leave(data.Player);

                var targetRoom = Rooms.Find(x => x.Data.ID == data.RoomId);
                var joiningPlayer = Players.Find(x => x.Data.ID == data.Player.ID);

                if (targetRoom != null)
                {
                    targetRoom.Join(joiningPlayer, data.Password);
                }
                else
                {
                    joiningPlayer.SendRoomError(RoomError.RoomDoesNotExist);
                }
            });

            Message.IsType<ClientMessage.LeaveRoom>(json, (data) =>
            {
                var containingRoom = Rooms.Find(x => x.HasPlayer(data.Player));

                if (containingRoom != null)
                {
                    Console.WriteLine("Remove {0} from room", data.Player.Name);
                    containingRoom.Leave(data.Player);
                }
                else
                {
                    Console.WriteLine("Cannot remove {0} from room as they are not in that room", data.Player.Name);
                }
            });

            Message.IsType<ClientMessage.CreateRoom>(json, (data) =>
            {
                Console.WriteLine("Create room for {0} with password {1}", data.Player.Name, data.Password);

                var playerCurrentRoom = FindRoomContainingPlayer(data.Player);
                if (playerCurrentRoom != null)
                    playerCurrentRoom.Leave(data.Player);

                var creator = Players.Find(x => x.Data.ID == data.Player.ID);
                var room = new Room(creator, data.Password);

                room.OnEmpty += OnRoomEmpty;

                Rooms.Add(room);
            });
        }

        void OnRoomEmpty(object sender, Room e)
        {
            Console.WriteLine("Closing room {0} as it is empty", e.Data.ID);
            e.OnEmpty -= OnRoomEmpty;
            Rooms.Remove(e);
        }

        public override void OnClose(IWebSocketConnection socket)
        {
            base.OnClose(socket);

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

        Room FindRoomContainingPlayer(PlayerData player)
        {
            return Rooms.Find(x => x.HasPlayer(player));
        }
    }
}
