using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace DrawesomeServer
{
    public class Chat : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.Write("Data: " + e.Data + ", Type: " + e.Type);
            Send("Echo...");
        }
    }
}
