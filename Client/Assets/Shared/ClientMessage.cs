using System;
using Protocol;

namespace Protocol
{
    public class ClientMessage
    {
        public class RequestConnection : Message
        {
            public string ID;
            public string Name;

            public RequestConnection(string id, string name)
            {
                Type = MessageType.ClientConnectionRequest;
                ID = id;
                Name = name;
            }
        }

        public class CreateRoom : Message
        {
            public PlayerData Player;
            public string Password;

            public CreateRoom(PlayerData player, string password)
            {
                Type = MessageType.ClientCreateRoom;
                Player = player;
                Password = password;
            }
        }

        public class JoinRoom : Message
        {
            public PlayerData Player;
            public string RoomId;
            public string Password;

            public JoinRoom(PlayerData player, string roomId, string password = "")
            {
                Type = MessageType.ClientJoinRoom;
                Player = player;
                RoomId = roomId;
                Password = password;
            }
        }

        public class LeaveRoom : Message
        {
            public PlayerData Player;

            public LeaveRoom(PlayerData player)
            {
                Type = MessageType.ClientLeaveRoom;
                Player = player;
            }
        }

        public class RequestRoomList : Message
        {
            public PlayerData Player;

            public RequestRoomList(PlayerData player)
            {
                Type = MessageType.ClientRequestRoomList;
                Player = player;
            }
        }
    }
}
