using System;
using System.Collections.Generic;
using System.Linq;
using Server.Game;
using Protocol;
using System.Collections.ObjectModel;

namespace Server.Drawesome
{
    public class DrawesomeGameData : GameData, ILogger
    {
        Queue<DrawingData> drawings = new Queue<DrawingData>();
        List<AnswerData> answers = new List<AnswerData>();
        Dictionary<Player, PromptData> prompts = new Dictionary<Player, PromptData>();
        Dictionary<PlayerData, uint> scores;

        string ILogger.LogName { get { return "Drawesome Game Controller"; } }

        List<PromptData> PromptPool { get; set; }
        DrawingData CurrentDrawing { get; set; }

        public DrawesomeGameData()
        {

        }

        public DrawesomeGameData(List<Player> players, Settings settings) : base(players, settings)
        {

        }

        /// <summary>
        /// Returns the latest scores for this round and the answers that each player gave (if any).
        /// </summary>
        /// <returns></returns>
        public Dictionary<PlayerData, ScoreData> GetLatestScores()
        {
            // Make dictionary of player's and their answer (if any)
            var playersWhoAnswered = new Dictionary<PlayerData, AnswerData>();
            foreach (var answer in Answers)
            {
                if (answer.Type == GameAnswerType.Player)
                {
                    playersWhoAnswered.Add(answer.Author, answer);
                }
            }

            // Try and get the player's answer, if they gave one
            var scoreData = new Dictionary<PlayerData, ScoreData>();
            foreach (var score in scores)
            {
                var answer = playersWhoAnswered.FirstOrDefault(x => x.Key == score.Key).Value;
                scoreData.Add(score.Key, new ScoreData(score.Value, answer));
            }

            return scoreData;
        }

        #region Submission Handlers

        public void SubmitAnswer(AnswerData answer)
        {
            Console.WriteLine("Add answer: {0}", answer.Answer);
            answers.Add(answer);
        }

        public void SubmitChoice(string chosenAnswer, Player player)
        {
            var answer = GetAnswer(chosenAnswer);

            Console.WriteLine("Player {0} chose {1}", player.Data.Name, answer.Answer);

            if (scores == null)
            {
                scores = new Dictionary<PlayerData, uint>();
                Players.ForEach(x => scores.Add(x.Data, 0));
            }

            if (answer.Type == GameAnswerType.ActualAnswer)
            {
                // Give the drawing's creator points
                scores[CurrentDrawing.Creator] += Settings.Drawesome.PointsToDrawerForCorrectAnswer;

                // Give the choosers points
                scores[player.Data] += Settings.Drawesome.PointsForCorrectAnswer;
            }
            else if (answer.Type == GameAnswerType.Player)
            {
                // Give the answer's author points
                scores[answer.Author] += Settings.Drawesome.PointsForFakeAnswer;

                // Assign points earned into answer. This value will be displayed for each result.
                answer.Points += Settings.Drawesome.PointsForFakeAnswer;
            }

            answer.Choosers.Add(player.Data);
        }

        public void SubmitDrawing(Player player, byte[] image)
        {
            // Find the prompt that was sent to the player
            var prompt = prompts[player];

            Console.WriteLine("Recieve image from {0} with {1} bytes for prompt {2}", player.Data.Name, image.Length, prompt.GetText());

            if (!drawings.Any(x => x.Creator.ID == player.Data.ID))
                drawings.Enqueue(new DrawingData(player.Data, image, prompt));
        }

        public void SubmitLike(string answer)
        {
            GetAnswer(answer).Likes++;
        }

        #endregion

        #region Send Data

        public void SendPromptsToPlayers()
        {
            foreach (var player in Players)
            {
                // Send prompt to player in lower-case.
                var prompt = GetPrompt(player);
                player.SendPrompt(prompt.GetText());
            }
        }

        public void SendDrawingToPlayers()
        {
            CurrentDrawing = drawings.Dequeue();
            Players.ForEach(x => x.SendImage(CurrentDrawing));
        }

        public void SendChoicesToPlayers()
        {
            // Find out how many players didn't answer
            var playersWithNoAnswerCount = 0;
            foreach (var player in Players)
            {
                if (!answers.Any(x => x.Author == player.Data) && player.Data != CurrentDrawing.Creator)
                    playersWithNoAnswerCount++;
            }

            // Add decoys for every player that didn't answer
            var decoys = Settings.Drawesome.Decoys;

            if (decoys.Count == 0)
            {
                Logger.Warn(this, "Can't add decoys as none are present in Drawesome's settings");
            }
            else
            {
                var rnd = new Random();
                for (int i = 0; i < playersWithNoAnswerCount; i++)
                {
                    var decoy = decoys[rnd.Next(0, decoys.Count - 1)];
                    answers.Add(new AnswerData(decoy, GameAnswerType.Decoy));
                    Console.WriteLine("Add decoy: {0}", decoy);
                }
            }

            // Add actual answer
            var answerData = new AnswerData(CurrentDrawing.Prompt.GetText(), GameAnswerType.ActualAnswer);
            answers.Add(answerData);

            Players.ForEach(x => x.SendChoices(CurrentDrawing.Creator, answers.ToList()));
        }

        #endregion

        /// TODO: eh?
        public void OnNewRound()
        {
            answers.Clear();
        }

        PromptData GetPrompt(Player player)
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

        #region Helpers

        public bool HasDrawings()
        {
            return drawings.Count != 0;
        }

        public List<Player> GetAnsweringPlayers()
        {
            return Players.Where(x => x.Data.ID != CurrentDrawing.Creator.ID).ToList();
        }

        public Queue<AnswerData> GetResults()
        {
            var results = new Queue<AnswerData>();

            // Sort answers by least chosen to most chosen excluding the actual answer
            var sortedAnswers = ChosenAnswers
                .Where(x => x.Type != GameAnswerType.ActualAnswer)
                .OrderBy(x => x.Choosers.Count)
                .ToList();
            sortedAnswers.ForEach(x => results.Enqueue(x));

            // Append the actual answer to the queue
            var actualAnswer = ChosenAnswers.FirstOrDefault(x => x.Type == GameAnswerType.ActualAnswer);
            if (actualAnswer == null)
                actualAnswer = new AnswerData(CurrentDrawing.Prompt.GetText(), GameAnswerType.ActualAnswer);
            results.Enqueue(actualAnswer);

            return results;
        }

        public bool IsPrompt(string answer)
        {
            // TODO: Replace string with object
            return prompts.Any(x => x.Value.GetText() == answer.ToLower());
        }

        public bool MatchesExistingAnswer(string answer)
        {
            return answers.Any(x => x.Answer.ToLower() == answer.ToLower());
        }

        public ReadOnlyCollection<AnswerData> ChosenAnswers
        {
            get
            {
                return new ReadOnlyCollection<AnswerData>(answers.Where(x => x.Choosers.Count != 0).ToList());
            }
        }

        public ReadOnlyCollection<DrawingData> Drawings
        {
            get
            {
                return new ReadOnlyCollection<DrawingData>(drawings.ToList());
            }
        }

        public ReadOnlyCollection<AnswerData> Answers
        {
            get
            {
                return new ReadOnlyCollection<AnswerData>(answers.ToList());
            }
        }

        #endregion
    }
}
