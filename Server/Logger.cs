using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    static class Logger
    {
        public static bool Enabled { get { return enabled; } set { enabled = value; } }
        static bool enabled = true;

        public static void WriteLine(string message, params object[] args)
        {
            if (enabled)
            {
                WriteLine(string.Format(message, args));
            }
        }

        static void WriteLine(string message)
        {
            if (enabled)
            {
                Console.WriteLine(message);
            }
        }
    }
}
