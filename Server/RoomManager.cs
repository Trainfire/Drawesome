using System;
using System.Collections.Generic;
using System.Linq;
using Protocol;

namespace Server
{
    public class RoomManager : IConnectionMessageHandler
    {
        Settings Settings { get; set; }
        List<Room> Rooms { get; set; }

        public RoomManager(ConnectionsHandler connectionHandler, Settings settings)
        {
            Rooms = new List<Room>();
            Settings = settings;
            connectionHandler.AddMessageListener(this);
        }

        void IConnectionMessageHandler.HandleMessage(Player player, string json)
        {
            Message.IsType<ClientMessage.RequestRoomList>(json, (data) =>
            {
                Console.WriteLine("Recieved a request from {0} for a list a rooms.", data.Player.ID);
                var rooms = Rooms.Select(x => x.RoomData).ToList();
                player.SendMessage(new ServerMessage.RoomList(rooms));
            });

            Message.IsType<ClientMessage.JoinRoom>(json, (data) =>
            {
                Console.WriteLine("Recieved a request from {0} to join room {1}.", data.Player.ID, data.RoomId);

                var roomHasPlayer = Rooms.Find(x => x.HasPlayer(data.Player));
                if (roomHasPlayer != null)
                    roomHasPlayer.Leave(data.Player);

                var targetRoom = Rooms.Find(x => x.RoomData.ID == data.RoomId);

                if (targetRoom != null)
                {
                    targetRoom.Join(player, data.Password);
                }
                else
                {
                    player.SendRoomError(RoomError.RoomDoesNotExist);
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

                Console.WriteLine("You shouldn't see this!");

                var playerCurrentRoom = FindRoomContainingPlayer(data.Player);
                if (playerCurrentRoom != null)
                    playerCurrentRoom.Leave(data.Player);

                var room = new Room(player, Settings, data.Password);

                room.OnEmpty += OnRoomEmpty;

                Rooms.Add(room);
            });
        }

        void OnRoomEmpty(object sender, Room e)
        {
            Console.WriteLine("Closing room {0} as it is empty", e.RoomData.ID);
            e.OnEmpty -= OnRoomEmpty;
            Rooms.Remove(e);
        }

        Room FindRoomContainingPlayer(PlayerData player)
        {
            return Rooms.Find(x => x.HasPlayer(player));
        }
    }
}
