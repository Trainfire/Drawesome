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
