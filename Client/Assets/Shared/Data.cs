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

    public class ChoiceData
    {
        public PlayerData Chooser { get; set; }
        public AnswerData ChosenAnswer { get; set; }

        public ChoiceData(PlayerData chooser, AnswerData answer)
        {
            Chooser = chooser;
            ChosenAnswer = answer;
        }
    }

    public class OptionsData
    {
        public List<AnswerData> Options { get; set; }
    }

    public class ResultData
    {
        public List<PlayerData> Players { get; set; }
        public int Likes { get; set; }
    }
}
