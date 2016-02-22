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

        /// <summary>
        /// Calculates the current score.
        /// </summary>
        /// <returns></returns>
        public void CalculateScores()
        {
            if (scores == null)
            {
                scores = new Dictionary<PlayerData, uint>();
                Players.ForEach(x => scores.Add(x.Data, 0));
            }

            // Give points for each chosen answer to the appropriate players.
            foreach (var answer in ChosenAnswers)
            {
                if (answer.Type == GameAnswerType.ActualAnswer)
                {
                    // Give the drawing's creator points
                    scores[CurrentDrawing.Creator] += (uint)answer.Choosers.Count * Settings.Drawesome.PointsToDrawerForCorrectAnswer;

                    // Give the choosers points
                    answer.Choosers.ForEach(x => scores[x] += Settings.Drawesome.PointsForCorrectAnswer);
                }
                else if (answer.Type == GameAnswerType.Player)
                {
                    // Give the answer's author points
                    var points = (uint)answer.Choosers.Count * Settings.Drawesome.PointsForFakeAnswer;
                    scores[answer.Author] += points;

                    // Assign points earned into answer. This value will be displayed for each result.
                    answer.Points = points;
                }
            }
        }

        /// <summary>
        /// Returns the latest scores for this round and the answers that each player gave (if any).
        /// </summary>
        /// <returns></returns>
        public Dictionary<PlayerData, ScoreData> GetLatestScores()
        {
            var answerData = new Dictionary<PlayerData, AnswerData>();
            foreach (var answer in Answers)
            {
                if (answer.Type == GameAnswerType.Player)
                {
                    answerData.Add(answer.Author, answer);
                }
            }

            var scoreData = new Dictionary<PlayerData, ScoreData>();

            foreach (var score in scores)
            {
                // Try and get the player's answer, if they gave one.
                var answer = answerData.FirstOrDefault(x => x.Key == score.Key).Value;
                scoreData.Add(score.Key, new ScoreData(score.Value, answer));
            }

            return scoreData;
        }

        public void AddAnswer(AnswerData answer)
        {
            Console.WriteLine("Add answer: {0}", answer.Answer);
            answers.Add(answer);
        }

        public void AddActualAnswer()
        {
            var answerData = new AnswerData(CurrentDrawing.Prompt.GetText(), GameAnswerType.ActualAnswer);
            answers.Add(answerData);
        }

        public void AddChosenAnswer(string answer, Player player)
        {
            var answerData = GetAnswer(answer);
            Console.WriteLine("Add chosen answer: {0}", answerData.Answer);
            answerData.Choosers.Add(player.Data);
        }

        /// <summary>
        /// Adds one decoy for every player that hasn't provided an answer.
        /// </summary>
        public void AddDecoys()
        {
            int decoyCount = 0;

            foreach (var player in Players)
            {
                if (!ChosenAnswers.Any(x => x.Author == player.Data) && player.Data != CurrentDrawing.Creator)
                    decoyCount++;
            }

            Console.WriteLine("{0} players did not submit an answer. Adding {0} decoys...", decoyCount);

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
            // Get prompts from JSON
            if (PromptPool == null)
                PromptPool = Settings.Prompts.Items;

            // Choose a random prompt from the pool
            var rnd = new Random();
            var index = rnd.Next(0, PromptPool.Count - 1);
            var prompt = PromptPool[index];
            PromptPool.Remove(prompt);

            // Make sure the prompt text is formatted if it contains a special token such as 'random player name'
            prompt.ReplaceTokens(player.Data, Players.Select(x => x.Data).ToList());

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
