using Fleck;
using Protocol;
using System.Linq;
using System;

namespace Server
{
    public enum PlayerCloseReason
    {
        None,
        Disconnected,
        Left,
        Kicked,
    }

    struct PlayerConnectionClosed
    {
        public Player Player { get; private set; }
        public PlayerCloseReason CloseReason { get; private set; }

        public PlayerConnectionClosed(Player player, PlayerCloseReason reason)
        {
            Player = player;
            CloseReason = reason;
        }
    }

    class Player : ProtocolPlayer
    {
        public event EventHandler<PlayerConnectionClosed> ConnectionClosed;

        public override string ID { get; set; }
        public override string Name { get; set; }
        public IWebSocketConnection Socket { get; private set; }

        public Player(string playerName, IWebSocketConnection socket)
        {
            Name = playerName;
            ID = Guid.NewGuid().ToString();
            Socket = socket;
            
            socket.OnClose += () =>
            {
                if (ConnectionClosed != null)
                    ConnectionClosed(this, new PlayerConnectionClosed(this, PlayerCloseReason.Disconnected));
            };

            socket.OnMessage += (message) =>
            {
                // TODO: Handle player leaving room here
                //if (ConnectionClosed != null)
                //    ConnectionClosed(this, new PlayerConnectionClosed(this, PlayerCloseReason.Left));
            };
        }

        public void SendMessage(Message message)
        {
            Socket.Send(message.AsJson());
        }

        public void Update(ClientConnectionsHandler manager)
        {
            Console.WriteLine("Send update to " + Name);
        }
    }
}
