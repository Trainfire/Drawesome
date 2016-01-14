using System;
using Json;

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
        public MessageType Type;

        public Message()
        {

        }

        public Message(MessageType type)
        {
            Type = type;
        }

#if !UNITY_5
        public T Deserialise<T>(string json)
        {
            return JsonParser.Deserialize<T>(json);
        }
#endif
    }

    // Data
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
}
