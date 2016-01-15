using System;

#if !UNITY_5
using Newtonsoft.Json;
#endif

namespace Protocol
{
    [Serializable]
    public enum MessageType
    {
        None,
        PlayerReady,
        ForceStartRound,
    }

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

    [System.Serializable]
    public class PlayerReadyMessage : Message
    {
        public bool IsReady;

        public PlayerReadyMessage(bool isReady)
        {
            Type = MessageType.PlayerReady;
            IsReady = isReady;
        }
    }

    #endregion
}
