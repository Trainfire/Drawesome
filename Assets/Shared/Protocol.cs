using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Protocol
{
#region Messages

    public class PlayerReadyMessage : Message
    {
        public bool IsReady;

        public PlayerReadyMessage(bool isReady)
        {
            Type = MessageType.PlayerReady;
            IsReady = isReady;
        }
    }

    public class PlayerConnectMessage : Message
    {
        public string ID;
        public string PlayerName;

        public PlayerConnectMessage(string id, string playerName)
        {
            Type = MessageType.PlayerConnect;
            PlayerName = playerName;
            ID = id;
        }
    }

    public class ValidatePlayer : Message
    {
        public string ID;

        public ValidatePlayer(string guid)
        {
            Type = MessageType.ValidatePlayer;
            ID = guid;
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

#endregion
}
