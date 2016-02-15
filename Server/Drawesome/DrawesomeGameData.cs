using System;
using System.Collections.Generic;
using System.Linq;
using Server.Game;
using Protocol;
using System.Collections.ObjectModel;

namespace Server.Drawesome
{
    public class DrawesomeGameData : GameData
    {
        Queue<DrawingData> drawings = new Queue<DrawingData>();
        List<AnswerData> answers = new List<AnswerData>();
        Dictionary<Player, PromptData> prompts = new Dictionary<Player, PromptData>();
        Dictionary<PlayerData, uint> scores = new Dictionary<PlayerData, uint>();

        List<PromptData> PromptPool { get; set; }
        public DrawingData CurrentDrawing { get; private set; }

        public DrawesomeGameData()
        {

        }

        public DrawesomeGameData(List<Player> players, Settings settings) : base(players, settings)
        {

        }

        public ReadOnlyCollection<DrawingData> Drawings
        {
            get
            {
                return new ReadOnlyCollection<DrawingData>(drawings.ToList());
            }
        }

        public ReadOnlyCollection<AnswerData> ChosenAnswers
        {
            get
            {
                return new ReadOnlyCollection<AnswerData>(answers.Where(x => x.Choosers.Count != 0).ToList());
            }
        }

        public ReadOnlyDictionary<Player, PromptData> SentPrompts
        {
            get
            {
                return new ReadOnlyDictionary<Player, PromptData>(prompts);
            }
        }

        public ReadOnlyDictionary<PlayerData, uint> Scores
        {
            get
            {
                return new ReadOnlyDictionary<PlayerData, uint>(scores);
            }
        }

        public ReadOnlyCollection<AnswerData> Answers
        {
            get
            {
                return new ReadOnlyCollection<AnswerData>(answers.ToList());
            }
        }

        public void AddPoints(PlayerData player, uint points)
        {
            if (scores == null)
            {
                scores = new Dictionary<PlayerData, uint>();
                Players.ForEach(x => scores.Add(x.Data, 0));
            }

            Console.WriteLine("Give {0} {1} points", player.Name, points);
            scores[player] += points;
        }

        public void AddAnswer(AnswerData answer)
        {
            Console.WriteLine("Add answer: {0}", answer.Answer);
            answers.Add(answer);
        }

        public void AddChosenAnswer(string answer, Player player)
        {
            var findAnswer = GetAnswer(answer);
            Console.WriteLine("Add chosen answer: {0}", findAnswer.Answer);
            findAnswer.Choosers.Add(player.Data);

            AddPoints(findAnswer.Author, Settings.Drawesome.PointsPerChoice);
            // TODO: Give more points if answer matches prompt
        }

        public void AddDrawing(DrawingData drawing)
        {
            drawings.Enqueue(drawing);
        }

        public void AddLike(string answer)
        {
            GetAnswer(answer).Likes++;
        }

        public void OnNewRound()
        {
            answers.Clear();
            if (drawings.Count != 0)
                drawings.Dequeue();
        }

        public DrawingData GetDrawing()
        {
            CurrentDrawing = drawings.Peek();
            return CurrentDrawing;
        }

        public bool HasDrawings()
        {
            return drawings.Count != 0;
        }

        public PromptData GetPrompt(Player player)
        {
            if (PromptPool == null)
                PromptPool = Settings.Prompts.Items;

            var rnd = new Random();
            var index = rnd.Next(0, PromptPool.Count - 1);
            var prompt = PromptPool[index];
            PromptPool.Remove(prompt);

            if (prompts == null)
                prompts = new Dictionary<Player, PromptData>();

            prompts.Add(player, prompt);

            return prompt;
        }

        AnswerData GetAnswer(string answer)
        {
            return answers.First(x => x.Answer == answer);
        }
    }
}
