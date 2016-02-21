using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Protocol
{
    [Serializable]
    public class Message
    {
        public string Identity;
        public string LogMessage;

        public Message()
        {
            Identity = GetType().ToString();
        }

        public Message(string logMessage)
        {
            LogMessage = logMessage;
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

        public static bool IsType<T>(string json, Action<T> OnTrue = null) where T : Message
        {
            if (string.IsNullOrEmpty(json))
            {
                Console.WriteLine("Recieved empty JSON");
                return false;
            }

            var obj = JsonHelper.FromJson<T>(json);

            if (obj != null && obj.Identity == typeof(T).FullName && OnTrue != null)
                OnTrue(obj);

            return obj.Identity == typeof(T).FullName;
        }
    }
}
