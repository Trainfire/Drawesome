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
        ServerCompleteConnectionRequest,
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
            public string PlayerID { get; private set; }

            public RequestRoomList(string playerId)
            {
                Type = MessageType.ClientRequestRoomList;
                PlayerID = playerId;
            }
        }
    }

    public class ServerMessage
    {
        public class ApproveClientConnection : Message
        {
            public string ID;

            public ApproveClientConnection(string guid)
            {
                Type = MessageType.ServerCompleteConnectionRequest;
                ID = guid;
            }
        }

        public class GetRoomList : Message
        {
            public string TargetPlayerID { get; private set; }
            public List<ProtocolRoom> Rooms { get; private set; }

            public GetRoomList(string targetPlayerId, List<ProtocolRoom> rooms)
            {
                Type = MessageType.ServerSendRoomList;
                TargetPlayerID = targetPlayerId;
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
