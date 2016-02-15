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
        public bool GameStarted { get; set; }

        public RoomData()
        {
            ID = "";
            Password = "";
            Owner = new PlayerData();
            Players = new List<PlayerData>();
            GameStarted = false;
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
        public int Likes { get; set; }
        public List<PlayerData> Choosers { get; set; }
        public uint Points { get; set; }

        public AnswerData()
        {
            Author = new PlayerData();
            Answer = string.Empty;
            Likes = 0;
            Choosers = new List<PlayerData>();
            Points = 0;
        }

        public AnswerData(string answer) : this()
        {
            Answer = answer;
        }

        public AnswerData(PlayerData author, string answer) : this()
        {
            Author = author;
            Answer = answer;
        }
    }

    public class OptionsData
    {
        public List<AnswerData> Options { get; set; }
    }

    public class ResultData
    {
        public PlayerData Author { get; private set; }
        public List<PlayerData> Players { get; private set; }
        public string Answer { get; private set; }
        public uint Points { get; private set; }
        public bool IsAnswer { get; private set; }

        public ResultData(PlayerData author, List<PlayerData> players, string answer, uint points, bool isAnswer)
        {
            Author = author;
            Players = players;
            Answer = answer;
            Points = points;
            IsAnswer = isAnswer;
        }
    }

    public class PromptData
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}
