using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Protocol
{
    #region Messages

    [Serializable]
    public enum MessageType
    {
        None = 0,
        Generic = 1,
        PlayerConnect = 2,
        PlayerJoined = 3,
        PlayerLeft = 4,
        ValidatePlayer = 5,
        PlayerReady = 6,
        ForceStartRound = 7,
        SendChatFromPlayer = 8,
        ServerUpdate = 9,
        PlayerAction = 10,
    }

    public class PlayerReadyMessage : Message
    {
        public bool IsReady;

        public PlayerReadyMessage(bool isReady)
        {
            Type = MessageType.PlayerReady;
            IsReady = isReady;
        }
    }

    public class PlayerFirstConnectMessage : Message
    {
        public string ID;
        public string PlayerName;

        public PlayerFirstConnectMessage(string id, string playerName)
        {
            Type = MessageType.PlayerConnect;
            PlayerName = playerName;
            ID = id;
        }
    }

    public class PlayerLeft : Message
    {
        public ProtocolPlayer Player = new ProtocolPlayer();

        public PlayerLeft(ProtocolPlayer player)
        {
            Type = MessageType.PlayerLeft;
            Player.ID = player.ID;
            Player.Name = player.Name;
        }
    }

    public class PlayerJoined : Message
    {
        public ProtocolPlayer Player = new ProtocolPlayer();

        public PlayerJoined(ProtocolPlayer player)
        {
            Type = MessageType.PlayerJoined;
            Player.ID = player.ID;
            Player.Name = player.Name;
        }
    }

    public class PlayerAction : Message
    {
        public ProtocolPlayer Player = new ProtocolPlayer();

        public PlayerAction(ProtocolPlayer player)
        {
            Type = MessageType.PlayerAction;
            Player.ID = player.ID;
            Player.Name = player.Name;
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
