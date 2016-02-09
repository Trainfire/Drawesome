using System;
using System.Collections.Generic;

namespace Protocol
{
    [Serializable]
    public class PlayerData
    {
        public string ID;
        public string Name;
    }

    public class RoomData
    {
        public string ID { get; set; }
        public string Password { get; set; }
        public List<PlayerData> Players { get; set; }
    }

    public class DrawingData
    {
        public PlayerData Creator { get; set; }
        public byte[] Image { get; set; }
    }

    public class GuessData
    {
        public PlayerData Author { get; set; }
        public string Guess { get; set; }
    }

    public class ChoiceData
    {
        public List<PlayerData> Players { get; set; }
        public int Likes { get; set; }
        public GuessData Guess { get; set; }
    }
}
