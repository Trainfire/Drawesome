using System;
using System.Collections.Generic;

namespace Protocol
{
    public class ProtocolInfo
    {
        public const uint Version = 2;
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
