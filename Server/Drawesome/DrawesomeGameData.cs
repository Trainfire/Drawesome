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
        Dictionary<PlayerData, uint> scores;

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

        public void AddPoints(AnswerData answer)
        {
            if (scores == null)
            {
                scores = new Dictionary<PlayerData, uint>();
                Players.ForEach(x => scores.Add(x.Data, 0));
            }

            if (answer.Type == GameAnswerType.Player)
            {
                Console.WriteLine("Give {0} {1} points", answer.Author.Name, Settings.Drawesome.PointsPerChoice);
                scores[answer.Author] += Settings.Drawesome.PointsPerChoice;
            }
            else if (answer.Type == GameAnswerType.ActualAnswer)
            {
                scores[CurrentDrawing.Creator] += Settings.Drawesome.PointsForCorrectAnswer;
            }
        }

        public void AddAnswer(AnswerData answer)
        {
            Console.WriteLine("Add answer: {0}", answer.Answer);
            answers.Add(answer);
        }

        public void AddActualAnswer()
        {
            var answerData = new AnswerData(CurrentDrawing.Prompt.Text, GameAnswerType.ActualAnswer);
            answers.Add(answerData);
        }

        public void AddChosenAnswer(string answer, Player player)
        {
            var answerData = GetAnswer(answer);
            Console.WriteLine("Add chosen answer: {0}", answerData.Answer);
            answerData.Choosers.Add(player.Data);

            AddPoints(answerData);
        }

        public void AddDecoys()
        {
            var decoyCount = Players.Count - ChosenAnswers.Count;
            var rnd = new Random();
            for (int i = 0; i < decoyCount; i++)
            {
                var decoy = Settings.Drawesome.Decoys[rnd.Next(0, Settings.Drawesome.Decoys.Count - 1)];
                answers.Add(new AnswerData(decoy, GameAnswerType.Decoy));
                Console.WriteLine("Add decoy: {0}", decoy);
            }
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
