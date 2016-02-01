using System;

namespace Protocol
{
    static class ProtocolHelper
    {
        public static bool EnableLogging = true;

        public static void LogMessage(string message)
        {
            if (!EnableLogging)
                return;

            var obj = JsonHelper.FromJson<Message>(message);
            Console.WriteLine(string.Format("{0}, Type: {1}, Log: {2}", DateTime.Now, obj.Type, obj.LogMessage));
        }

        public static void LogMessage(Message message)
        {
            if (!EnableLogging)
                return;

            var log = string.Format("{0}, Type: {1}, Log: {2}", DateTime.Now, message.Type, message.LogMessage);
            Console.WriteLine(log);
        }
    }
}
