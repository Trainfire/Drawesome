using System;
using System.Collections.Generic;

namespace Protocol
{
    public class PlayerData
    {
        public uint RoomId { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }

        public PlayerData()
        {
            RoomId = 0;
            ID = "";
            Name = "";
        }
    }

    public class RoomData
    {
        public string ID { get; set; }
        public string Password { get; set; }
        public PlayerData Owner { get; set; }
        public List<PlayerData> Players { get; set; }

        public RoomData()
        {
            ID = "";
            Password = "";
            Owner = new PlayerData();
            Players = new List<PlayerData>();
        }
    }

    public class DrawingData
    {
        public PlayerData Creator { get; set; }
        public byte[] Image { get; set; }
        public PromptData Prompt { get; set; }

        public DrawingData(PlayerData creator, byte[] image, PromptData prompt)
        {
            Creator = creator;
            Image = image;
            Prompt = prompt;
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
        public uint Points { get; set; }

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

    public class PromptData
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}
