using WebSocketSharp;
namespace Server
{
    class Player
    {
        public string Name { get; set; }
        public WebSocket Socket { get; set; }

        public Player(string playerName)
        {
            Name = playerName;
        }
    }
}
