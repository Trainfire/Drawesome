using System;
using System.Collections.Generic;
using System.Linq;
using Protocol;

namespace Server
{
    public class RoomManager : IConnectionMessageHandler, ILogger
    {
        SettingsLoader SettingsLoader { get; set; }
        List<Room> Rooms { get; set; }
        ConnectionsHandler ConnectionsHandler { get; set; }

        public string LogName { get { return "Room Manager"; } }

        public RoomManager(ConnectionsHandler connectionHandler, SettingsLoader settings)
        {
            Rooms = new List<Room>();
            SettingsLoader = settings;
            ConnectionsHandler = connectionHandler;
            ConnectionsHandler.AddMessageListener(this);
        }

        void IConnectionMessageHandler.HandleMessage(Player player, string json)
        {
            Message.IsType<ClientMessage.RequestRoomList>(json, (data) =>
            {
                Logger.Log(this, "Recieved a request from {0} for a list a rooms.", player);
                var rooms = Rooms.Select(x => x.RoomData).ToList();
                player.SendMessage(new ServerMessage.RoomList(rooms));
            });

            Message.IsType<ClientMessage.JoinRoom>(json, (data) =>
            {
                Logger.Log(this, "Recieved a request from {0} to join room {1}.", player.Data, data.RoomId);

                var roomHasPlayer = Rooms.Find(x => x.HasPlayer(player.Data));
                if (roomHasPlayer != null)
                    roomHasPlayer.Leave(player.Data);

                var targetRoom = Rooms.Find(x => x.RoomData.ID == data.RoomId);

                if (targetRoom != null)
                {
                    targetRoom.Join(player, data.Password);
                }
                else
                {
                    player.SendRoomJoinNotice(RoomNotice.RoomDoesNotExist);
                }
            });

            Message.IsType<ClientMessage.LeaveRoom>(json, (data) =>
            {
                var containingRoom = Rooms.Find(x => x.HasPlayer(player.Data));

                if (containingRoom != null)
                {
                    Logger.Log(this, "Remove {0} from room", player);
                    containingRoom.Leave(player.Data);
                }
                else
                {
                    Logger.Warn(this, "Cannot remove {0} from room as they are not in that room", player);
                }
            });

            Message.IsType<ClientMessage.CreateRoom>(json, (data) =>
            {
                if (Rooms.Count != SettingsLoader.Values.Server.MaxRooms)
                {
                    Logger.Log(this, "Create room for {0} with password {1}", player, data.Password);

                    var playerCurrentRoom = FindRoomContainingPlayer(player.Data);
                    if (playerCurrentRoom != null)
                        playerCurrentRoom.Leave(player.Data);

                    var room = new Room(ConnectionsHandler, player, SettingsLoader, data.Password);

                    room.OnEmpty += OnRoomEmpty;

                    Rooms.Add(room);
                }
                else
                {
                    Logger.Log(this, "Cannot create room for {0} as the maximum room limit has been reached", player);
                    player.SendRoomJoinNotice(RoomNotice.MaxRoomsLimitReached);
                }
            });
        }

        void OnRoomEmpty(object sender, Room e)
        {
            e.OnEmpty -= OnRoomEmpty;
            Rooms.Remove(e);
        }

        Room FindRoomContainingPlayer(PlayerData player)
        {
            return Rooms.Find(x => x.HasPlayer(player));
        }
    }
}
