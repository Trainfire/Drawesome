using System;
using System.Collections.Generic;

namespace Protocol
{
    [Serializable]
    public class PlayerData
    {
        public uint RoomId;
        public string ID;
        public string Name;

        public PlayerData()
        {
            RoomId = 0;
            ID = "";
            Name = "";
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as PlayerData;
            return other != null && other.ID == this.ID;
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }

    [Serializable]
    public class RoomData
    {
        public string ID;
        public string Password;
        public PlayerData Owner;
        public List<PlayerData> Players;
        public bool GameStarted;

        public RoomData()
        {
            ID = "";
            Password = "";
            Owner = new PlayerData();
            Players = new List<PlayerData>();
            GameStarted = false;
        }
    }

    [Serializable]
    public class DrawingData
    {
        public PlayerData Creator;
        public byte[] Image;
        public PromptData Prompt;

        public DrawingData(PlayerData creator, byte[] image, PromptData prompt)
        {
            Creator = creator;
            Image = image;
            Prompt = prompt;
        }
    }

    [Serializable]
    public class AnswerData
    {
        public PlayerData Author;
        public string Answer;
        public int Likes;
        public List<PlayerData> Choosers;
        public uint Points;
        public GameAnswerType Type;

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

        public AnswerData(string answer, GameAnswerType type) : this(answer)
        {
            Type = type;
        }

        public AnswerData(PlayerData author, string answer) : this()
        {
            Author = author;
            Answer = answer;
        }
    }

    [Serializable]
    public class PromptData
    {
        public int Id;
        public string Text;
    }
}
