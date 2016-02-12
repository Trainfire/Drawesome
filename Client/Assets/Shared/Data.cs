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

        public DrawingData(PlayerData creator, byte[] image)
        {
            Creator = creator;
            Image = image;
        }
    }

    public class AnswerData
    {
        public PlayerData Author { get; set; }
        public string Answer { get; set; }

        public AnswerData(PlayerData author, string answer)
        {
            Author = author;
            Answer = answer;
        }
    }

    public class OptionsData
    {
        public List<AnswerData> Options { get; set; }
    }

    public class ChoiceData
    {
        public List<PlayerData> Players { get; set; }
        public int Likes { get; set; }

        public ChoiceData()
        {
            Players = new List<PlayerData>();
        }
    }

    public class ResultData
    {
        public PlayerData Author { get; private set; }
        public List<PlayerData> Players { get; private set; }
        public string Answer { get; private set; }
        public uint Points { get; private set; }

        public ResultData(PlayerData author, List<PlayerData> players, string answer, uint points)
        {
            Author = author;
            Players = players;
            Answer = answer;
            Points = points;
        }
    }
}
