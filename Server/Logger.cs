using System;
using System.IO;

namespace Server
{
    static class Logger
    {
        public static bool Enabled { get { return enabled; } set { enabled = value; } }
        static bool enabled = true;

        public static void Log(ILogger logger, string message, params object[] args)
        {
            Log(logger.LogName + " - " + message, args);
        }

        public static void Warn(ILogger logger, string message, params object[] args)
        {
            Log("[WARNING] " + logger.LogName + " - " + message, args);
        }

        public static void Log(string message, params object[] args)
        {
            Log(string.Format(message, args));
        }

        static void Log(string message)
        {
            if (enabled)
            {
                var str = string.Format("{0} - {1}", DateTime.Now, message);
                Console.WriteLine(str);
                WriteToFile(str);
            }
        }

        static void WriteToFile(string log)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "log";

            if (!File.Exists(path))
            {
                var newFile = File.CreateText(path);
            }

            using (var sw = File.AppendText(path))
            {
                sw.WriteLine(log);
            }
        }
    }
}
