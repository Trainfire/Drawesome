using Fleck;

namespace Server
{
    class Player
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

        public void AssignSocket(IWebSocketConnection socket)
        {
            Socket = socket;
        }
    }
}
