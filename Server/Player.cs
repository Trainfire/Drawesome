using Fleck;
using Protocol;
using System.Linq;
using System;

namespace Server
{
    class Player : ProtocolPlayer
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public IWebSocketConnection Socket { get; private set; }

        public Player(string playerName, IWebSocketConnection socket)
        {
            Name = playerName;
            ID = System.Guid.NewGuid().ToString();
            Socket = socket;
        }

        public void SendMessage(Message message)
        {
            Socket.Send(message.AsJson());
        }

        public void Update(PlayerManager manager)
        {
            Console.WriteLine("Send update to " + Name);
        }
    }
}
