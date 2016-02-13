using Server.Game;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Timers;
using Protocol;
using System.Linq;

namespace Server.Drawesome
{
    public class DrawesomeGameData : GameData
    {
        public Queue<DrawingData> Drawings { get; private set; }
        public List<AnswerData> SubmittedAnswers { get; private set; }
        public Dictionary<AnswerData, ChoiceData> ChosenAnswers { get; private set; }
        public Dictionary<Player, PromptData> SentPrompts { get; private set; }

        List<PromptData> PromptPool { get; set; }

        public DrawesomeGameData()
        {
            Drawings = new Queue<DrawingData>();
            SubmittedAnswers = new List<AnswerData>();
            ChosenAnswers = new Dictionary<AnswerData, ChoiceData>();
        }

        public DrawesomeGameData(Settings settings) : base(settings)
        {
            
        }

        /// <summary>
        /// Returns a random prompt from a pool then removes it.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public PromptData GetPrompt(Player player)
        {
            if (PromptPool == null)
                PromptPool = Settings.Prompts.Items;

            var rnd = new Random();
            var index = rnd.Next(0, PromptPool.Count - 1);
            var prompt = PromptPool[index];
            PromptPool.Remove(prompt);

            if (SentPrompts == null)
                SentPrompts = new Dictionary<Player, PromptData>();

            SentPrompts.Add(player, prompt);

            return prompt;
        }
    }

    public class DrawesomeGame : Game<DrawesomeGameData>
    {
        DrawesomeGameData Data { get; set; }

        protected override string Name { get { return "Drawesome"; } }

        public DrawesomeGame(Settings settings) : base(settings)
        {
            AddState(GameState.RoundBegin, new StateRoundBegin());
            AddState(GameState.Drawing, new StateDrawingPhase());
            AddState(GameState.Answering, new StateAnsweringPhase());
            AddState(GameState.Choosing, new StateChoosingPhase());
            AddState(GameState.Results, new StateResultsPhase());
            AddState(GameState.RoundEnd, new StateRoundEnd());
        }

        public override void Start(List<Player> players)
        {
            base.Start(players);
            Log("Started");
            Data = new DrawesomeGameData(Settings);
            SetState(GameState.RoundBegin, Data);
        }

        protected override void OnEndState(DrawesomeGameData gameData)
        {
            // Loop game if drawings remain
            switch (CurrentState.Type)
            {
                case GameState.RoundBegin:
                    SetState(GameState.Drawing, gameData);
                    break;

                case GameState.Drawing:
                    SetState(GameState.Answering, gameData);
                    break;

                case GameState.Answering:

                    Console.WriteLine("Answers");
                    foreach (var answer in gameData.SubmittedAnswers)
                    {
                        Console.WriteLine("\t{0}: {1}", answer.Author.Name, answer.Answer);
                    }

                    SetState(GameState.Choosing, gameData);
                    break;

                case GameState.Choosing:

                    Console.WriteLine("Choices");
                    foreach (var choice in gameData.ChosenAnswers)
                    {
                        var players = choice.Value.Players
                            .Select(x => x.Name)
                            .Aggregate((current, next) => current + ", " + next);

                        Console.WriteLine("\t{0}: {1}", choice.Key.Answer, players);
                    }

                    SetState(GameState.Results, gameData);
                    break;

                case GameState.Results:
                    SetState(GameState.RoundEnd, gameData);
                    break;

                case GameState.RoundEnd:
                    if (gameData.Drawings.Count != 0)
                    {
                        gameData.Drawings.Dequeue();
                        SetState(GameState.Drawing, gameData);
                    }
                    else
                    {
                        Log("Game Over!");
                    }
                    break;

                default:
                    break;
            }
        }

        protected override bool IsGameOver()
        {
            return false;
        }
    }

    #region States

    public class StateRoundBegin : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.RoundBegin; } }

        protected override void OnBegin()
        {
            base.OnBegin();
            SetTimer("Begin Timer", GameData.Settings.Drawesome.RoundBeginTime, true);
        }
    }

    public class StateDrawingPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Drawing; } }

        public StateDrawingPhase()
        {
            
        }

        protected override void OnBegin()
        {
            SetTimer("Drawing Timer", GameData.Settings.Drawesome.DrawTime, true);

            // Send random prompts to players
            foreach (var player in GameData.Players)
            {
                player.SendPrompt(GameData.GetPrompt(player).Text);
            }
        }

        public override void OnPlayerMessage(Player player, string json)
        {
            Message.IsType<ClientMessage.Game.SendImage>(json, (data) =>
            {
                var prompt = GameData.SentPrompts[player];

                // Update GameData
                GameData.Drawings.Enqueue(new DrawingData(player.Data, data.Image, prompt));

                Console.WriteLine("Recieve image from {0} with {1} bytes for prompt {2}", player.Data.Name, data.Image.Length, prompt.Text);

                // Tell all clients that player has submitted drawing
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data));

                if (GameData.Drawings.Count == GameData.Players.Count)
                    EndState();
            });
        }
    }

    public class StateAnsweringPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Answering; } }

        public StateAnsweringPhase()
        {

        }

        protected override void OnBegin()
        {
            SetTimer("Answering Timer", GameData.Settings.Drawesome.AnsweringTime, true);
            var drawing = GameData.Drawings.Dequeue();
            GameData.Players.ForEach(x => x.SendImage(drawing.Image));
        }

        public override void OnPlayerMessage(Player player, string json)
        {
            Message.IsType<ClientMessage.Game.SubmitAnswer>(json, (data) =>
            {
                Console.WriteLine("{0} submitted '{1}'", player.Data.Name, data.Answer);
              
                if (IsPrompt(data.Answer))
                {
                    Console.WriteLine("Player {0}'s answer matches prompt!", player.Data.Name);
                    player.SendAnswerValidation(GameAnswerError.MatchesPrompt);
                }
                else if (HasPromptBeenSubmitted(data.Answer))
                {
                    Console.WriteLine("Player {0}'s answer matches an existing answer from another player!", player.Data.Name);
                    player.SendAnswerValidation(GameAnswerError.AlreadyExists);
                }        
                else
                {
                    player.SendAnswerValidation(GameAnswerError.None);

                    // Add answer here
                    GameData.SubmittedAnswers.Add(new AnswerData(player.Data, data.Answer));

                    // Tell all clients that player has submitted answer
                    GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data));

                    if (GameData.SubmittedAnswers.Count == GameData.Players.Count)
                        EndState();
                }
            });
        }

        bool IsPrompt(string answer)
        {
            return GameData.SentPrompts.Any(x => x.Value.Text.ToLower() == answer.ToLower());
        }

        bool HasPromptBeenSubmitted(string answer)
        {
            return GameData.SubmittedAnswers.Any(x => x.Answer.ToLower() == answer.ToLower());
        }
    }

    public class StateChoosingPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Choosing; } }

        public StateChoosingPhase()
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();

            SetTimer("Choosing Timer", GameData.Settings.Drawesome.ChoosingTime);

            // Send options to each player
            GameData.Players.ForEach(x => x.SendChoices(GameData.SubmittedAnswers));

            // Add answers here
            foreach (var answer in GameData.SubmittedAnswers)
            {
                GameData.ChosenAnswers.Add(answer, new ChoiceData());
            }
        }

        public override void OnPlayerMessage(Player player, string json)
        {
            var message = JsonHelper.FromJson<Message>(json);

            Message.IsType<ClientMessage.Game.SubmitChoice>(json, (data) =>
            {
                // Add answer
                var answer = GameData.SubmittedAnswers.Find(x => x.Answer == data.Choice);
                GameData.ChosenAnswers[answer].Players.Add(player.Data);

                // Tell all clients of choice
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data));

                // End state if all players have chosen
                if (GameData.ChosenAnswers.Count == GameData.Players.Count)
                    EndState();
            });

            Message.IsType<ClientMessage.Game.LikeAnswer>(json, (data) =>
            {
                // Add like
                var answer = GameData.SubmittedAnswers.Find(x => x.Answer == data.Answer);
                GameData.ChosenAnswers[answer].Likes++;
            });
        }

        protected override void OnTimerFinish(object sender, EventArgs e)
        {
            // TODO: Add decoys
            // ...

            base.OnTimerFinish(sender, e);
        }
    }

    public class StateResultsPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Results; } }

        Queue<KeyValuePair<AnswerData, ChoiceData>> ChosenAnswersQueue { get; set; }

        public StateResultsPhase()
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();

            // Sort chosen answers here...
            ChosenAnswersQueue = new Queue<KeyValuePair<AnswerData, ChoiceData>>();

            var sortedAnswers = GameData.ChosenAnswers.OrderBy(x => x.Value.Players.Count).ToList();
            foreach (var answer in sortedAnswers)
            {
                ChosenAnswersQueue.Enqueue(new KeyValuePair<AnswerData, ChoiceData>(answer.Key, answer.Value));
            }

            ShowNextResult();
        }

        void ShowNextResult()
        {
            // Send choice to client and remove from queue
            var chosenAnswer = ChosenAnswersQueue.Dequeue();
            var result = new ResultData(chosenAnswer.Key.Author, chosenAnswer.Value.Players, chosenAnswer.Key.Answer, 0);
            var message = new ServerMessage.Game.SendResult(result);
            GameData.Players.ForEach(x => x.SendMessage(message));

            // Start timer
            var timer = new GameTimer(5f);
            timer.Finish += UpdateQueue;
        }

        void UpdateQueue(object sender, EventArgs e)
        {
            if (ChosenAnswersQueue.Count != 0)
            {
                ShowNextResult();
            }
            else
            {
                EndState();
            }
        }
    }

    public class StateRoundEnd : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.RoundEnd; } }
    }

    #endregion
}
