using System;

#if !UNITY_5
using Newtonsoft.Json;
#endif

namespace Protocol
{
    public class Message
    {
        public string LogMessage;
        public MessageType Type;

        public Message()
        {

        }

        public Message(MessageType type, string logMessage = "")
        {
            LogMessage = logMessage;
            Type = type;
        }

#if !UNITY_5
        public T Deserialise<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
#endif

        public string AsJson()
        {
#if UNITY_5
            return UnityEngine.JsonUtility.ToJson(this);
#else
            return JsonConvert.SerializeObject(this);
#endif
        }
    }

    [Serializable]
    public class Player
    {
        public string Name { get; private set; }

        public Player()
        {

        }

        public Player(string name)
        {
            Name = name;
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }

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

    public class PlayerDisconnectMessage : Message
    {
        
    }

    public class PlayerSendChatMessage : MessageHandler
    {
        public string ID;
    }

#endregion
}
