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
            public string PlayerName;

            public RequestConnection(string id, string playerName)
            {
                Type = MessageType.ClientConnectionRequest;
                PlayerName = playerName;
                ID = id;
            }
        }

        public class JoinRoom : Message
        {
            public Guid PlayerID { get; private set; }
            public Guid RoomID { get; private set; }

            public JoinRoom(Guid playerId, Guid roomId)
            {
                PlayerID = playerId;
                RoomID = roomId;
            }
        }

        public class RequestRoomList : Message
        {
            public string PlayerID;

            public RequestRoomList(string playerId)
            {
                Type = MessageType.ClientRequestRoomList;
                PlayerID = playerId;
            }
        }
    }

    public class ServerMessage
    {
        public class ConnectionSuccess : Message
        {
            public string ID;

            public ConnectionSuccess(string guid)
            {
                Type = MessageType.ServerConnectionSuccess;
                ID = guid;
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
    }

    public class ServerUpdate : Message
    {
        public List<ProtocolPlayer> Players = new List<ProtocolPlayer>();

        public ServerUpdate()
        {
            Type = MessageType.ServerUpdate;
        }

        public ServerUpdate(List<ProtocolPlayer> players)
        {
            Type = MessageType.ServerUpdate;
            Players = players;
        }
    }
}
