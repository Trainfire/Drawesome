using System;
using WebSocketSharp;

#if !UNITY_5
using Newtonsoft.Json;
#endif

namespace Protocol
{
    static class ProtocolHelper
    {
        public static void LogMessage(MessageEventArgs e)
        {
#if !UNITY_5
            var message = JsonConvert.DeserializeObject<Message>(e.Data);
            Console.WriteLine(string.Format("{0}, Type: {1}, Log: {2}", DateTime.Now, message.Type, message.LogMessage));
#endif
        }

        public static void LogMessage(Message message)
        {
            var log = string.Format("{0}, Type: {1}, Log: {2}", DateTime.Now, message.Type, message.LogMessage);
            Console.WriteLine(log);
        }
    }
}
