using System;
using System.Collections.Generic;

namespace Protocol
{
    public class ProtocolInfo
    {
        public const uint Version = 2;
    }

    public class Log : Message
    {
        public string Message;

        public Log(string message)
        {
            Message = message;
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

        }

        public ServerUpdate(List<PlayerData> players)
        {
            Players = players;
        }
    }
}
