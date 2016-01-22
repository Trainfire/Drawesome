using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using Protocol;

namespace Server
{
    public class WebSocketBehaviour
    {
        public MessageHandler MessageHandler { get; private set; }

        public virtual void OnOpen(IWebSocketConnection socket)
        {

        }

        public virtual void OnClose(IWebSocketConnection socket)
        {

        }

        public virtual void OnMessage(string message)
        {
            
        }

        public virtual void OnError(Exception socket)
        {

        }
    }
}
