using System;
using System.Collections.Generic;
using System.Linq;

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
        public int Likes { get; private set; }
        public List<PlayerData> Choosers { get; set; }
        public uint Points { get; set; }
        public GameAnswerType Type { get; set; }

        public AnswerData()
        {
            Author = new PlayerData();
            Answer = string.Empty;
            Choosers = new List<PlayerData>();
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

        public void AddLike()
        {
            Likes += 1;
        }
    }

    public class PromptData
    {
        public int Id { get; set; }
        public string Text { private get; set; }

        // Replaces any tokens such as "random player name" appropriately replaced.
        public string ReplaceTokens(PlayerData player, List<PlayerData> players)
        {
            // TODO: Move somewhere more appropriate.
            var tokenPlayerName = "<RANDOM PLAYER NAME>";

            if (Text.Contains(tokenPlayerName))
            {
                // Replace with random player that isn't this player
                var playersExceptOwner = players.Where(x => x != player).ToList();

                var rnd = new Random().Next(playersExceptOwner.Count);
                Text = Text.Replace(tokenPlayerName, playersExceptOwner[rnd].Name.ToLower());
            }

            return Text;
        }

        /// <summary>
        /// Returns the text formatted to lower case.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return Text.ToLower();
        }
    }

    public class ScoreData
    {
        public uint Score { get; private set; }
        public uint Likes { get; private set; }
        public AnswerData AnswerGiven { get; private set; }

        public ScoreData(uint score, uint likes, AnswerData answerGiven)
        {
            Score = score;
            Likes = likes;
            AnswerGiven = answerGiven;
        }
    }
}
