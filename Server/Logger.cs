using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                Console.WriteLine(DateTime.Now + " - " + message);
        }
    }
}
