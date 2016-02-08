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
        ServerNotifyRoomError,
        Chat,
        ServerRoomUpdate,
    }

    public class Log : Message
    {
        public string Message;

        public Log(string message)
        {
            Type = MessageType.Log;
            Message = message;
        }
    }

    public class DrawesomeMessage
    {
        public class StateChange : Message
        {
            public GameState GameState;

            public StateChange(GameState gameState)
            {
                GameState = gameState;
            }
        }
    }

    public class SharedMessage
    {
        public class Chat : Message
        {
            public PlayerData Player;
            public string Message;

            public Chat(PlayerData player, string message)
            {
                Type = MessageType.Chat;
                Player = player;
                Message = message;
            }
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
            public PlayerData Player;
            public PlayerAction Action;

            public NotifyPlayerAction(PlayerData player, PlayerAction action)
            {
                Type = MessageType.ServerNotifyPlayerAction;
                Player = player;
                Action = action;
            }
        }

        public class NotifyRoomError : Message
        {
            public RoomError Notice;

            public NotifyRoomError(RoomError notice)
            {
                Type = MessageType.ServerNotifyRoomError;
                Notice = notice;
            }
        }

        public class RoomUpdate : Message
        {
            public RoomData RoomData;

            public RoomUpdate(RoomData roomData)
            {
                Type = MessageType.ServerRoomUpdate;
                RoomData = roomData;
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
