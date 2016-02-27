using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <summary>
        /// The image encoded as a base64 string.
        /// </summary>
        public string Image;
        public PlayerData Creator;
        public PromptData Prompt;

        public DrawingData(PlayerData creator, byte[] image, PromptData prompt)
        {
            Creator = creator;
            Image = Convert.ToBase64String(image);
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

    [Serializable]
    public class PromptData
    {
        public int Id;
        public string Text;

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

    [Serializable]
    public class ScoreData
    {
        public uint Score;
        public uint Likes;
        public AnswerData AnswerGiven;

        public ScoreData(uint score, uint likes, AnswerData answerGiven)
        {
            Score = score;
            Likes = likes;
            AnswerGiven = answerGiven;
        }
    }
}
