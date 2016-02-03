using System;
using System.Collections.Generic;

namespace Protocol
{
    [Serializable]
    public enum MessageType
    {
        None,
        Log,
        ClientConnectionRequest,
        ServerConnectionSuccess,
        ServerUpdate,
        ClientRequestRoomList,
        ServerSendRoomList,
        ServerNotifyPlayerAction,
        ClientJoinRoom,
        ClientLeaveRoom,
        ClientCreateRoom,
        ServerRoomJoinError,
    }

    public class Log : Message
    {
        public string Message { get; private set; }

        public Log(string message)
        {
            Type = MessageType.Log;
            Message = message;
        }

        public Log(string message, params object[] args)
        {
            Type = MessageType.Log;
            Message = string.Format(message, args);
        }
    }

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

            public JoinRoom(PlayerData player, string roomId)
            {
                Type = MessageType.ClientJoinRoom;
                Player = player;
                RoomId = roomId;
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

    public class ServerMessage
    {
        public class ConnectionSuccess : Message
        {
            public string ID;

            public ConnectionSuccess(string id)
            {
                Type = MessageType.ServerConnectionSuccess;
                ID = id;
            }
        }

        public class RoomList : Message
        {
            public List<RoomData> Rooms { get; private set; }

            public RoomList(List<RoomData> rooms)
            {
                Type = MessageType.ServerSendRoomList;
                Rooms = rooms;
            }
        }

        public class NotifyPlayerAction : Message
        {
            public PlayerAction Action;

            public NotifyPlayerAction(PlayerAction action)
            {
                Type = MessageType.ServerNotifyPlayerAction;
                Action = action;
            }
        }

        public class RoomJoinError : Message
        {
            public RoomError Error;

            public RoomJoinError(RoomError error)
            {
                Type = MessageType.ServerRoomJoinError;
                Error = error;
            }
        }
    }

    public class ServerUpdate : Message
    {
        public List<PlayerData> Players = new List<PlayerData>();

        public ServerUpdate()
        {
            Type = MessageType.ServerUpdate;
        }

        public ServerUpdate(List<PlayerData> players)
        {
            Type = MessageType.ServerUpdate;
            Players = players;
        }
    }
}
