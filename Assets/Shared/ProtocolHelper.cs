using System;

#if !UNITY_5
using Newtonsoft.Json;
using Fleck;
#endif

namespace Protocol
{
    static class ProtocolHelper
    {
        public static bool EnableLogging = true;

        public static void LogMessage(string message)
        {
#if !UNITY_5
            if (!EnableLogging)
                return;

            var obj = JsonConvert.DeserializeObject<Message>(message);
            Console.WriteLine(string.Format("{0}, Type: {1}, Log: {2}", DateTime.Now, obj.Type, obj.LogMessage));
#endif
        }

        public static void LogMessage(Message message)
        {
            if (!EnableLogging)
                return;

            var log = string.Format("{0}, Type: {1}, Log: {2}", DateTime.Now, message.Type, message.LogMessage);
            Console.WriteLine(log);
        }

#if !UNITY_5
        public static void Send(IWebSocketConnection socket, Message message)
        {
            var json = JsonConvert.SerializeObject(message);
            socket.Send(json);
        }
#endif

        public static Message GetMessage(string json)
        {
            return GetMessage<Message>(json);
        }

        public static T GetMessage<T>(string json) where T : Message
        {
            T message = JsonHelper.FromJson<T>(json);

            if (message != null)
            {
                return message;
            }
            else
            {
                Console.WriteLine("Failed to deserialize JSON into Message...");
                return null;
            }
        }
    }
}
