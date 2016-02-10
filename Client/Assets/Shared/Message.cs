using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Protocol
{
    public class Message
    {
        public string LogMessage { get; protected set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type;

        public Message()
        {
            Type = MessageType.None;
        }

        public Message(MessageType type, string logMessage = "")
        {
            LogMessage = logMessage;
            Type = type;
        }

        public T Deserialise<T>(string json)
        {
            return JsonHelper.FromJson<T>(json);
        }

        public string AsJson()
        {
            return JsonHelper.ToJson(this);
        }
    }
}
