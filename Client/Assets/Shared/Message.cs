using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Protocol
{
    public class Message
    {
        public string Name { get; set; }
        public string LogMessage { get; protected set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type;

        public Message()
        {
            Type = MessageType.None;
            Name = GetType().ToString();
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
            var json = JsonHelper.ToJson(this);
            return json;
        }

        public static bool IsType<T>(string json, Action<T> OnMatchingTypes = null) where T : Message
        {
            var obj = JsonHelper.FromJson<T>(json);

            if (obj != null && obj.Name == typeof(T).FullName && OnMatchingTypes != null)
                OnMatchingTypes(obj);

            return obj.Name == typeof(T).FullName;
        }
    }
}
