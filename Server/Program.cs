using System;
using System.Collections.Generic;
using Protocol;
using Newtonsoft.Json;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            server.Start();
        }
    }
}
