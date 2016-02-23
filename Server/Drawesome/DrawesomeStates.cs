using System;
using System.Collections.Generic;
using System.Linq;
using Server.Game;
using Protocol;

namespace Server.Drawesome
{
    public class StatePreGame : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.PreGame; } }

        public StatePreGame(Settings settings) : base(settings)
        {

        }
    }

    public class StateRoundBegin : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.RoundBegin; } }

        public StateRoundBegin(Settings settings) : base(settings)
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();
            SetCountdownTimer("Begin Timer", GameData.Settings.Drawesome.RoundBeginTime, false);
        }
    }

    public class StateDrawingPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Drawing; } }

        public StateDrawingPhase(Settings settings) : base(settings)
        {

        }

        protected override void OnBegin()
        {
            SetCountdownTimer("Drawing Timer", GameData.Settings.Drawesome.DrawTime, true);

            // Send random prompts to players
            foreach (var player in GameData.Players)
            {
                ResponseHandler.AddRespondant(player);
            }

            GameData.SendPromptsToPlayers();
        }

        public override void HandleMessage(Player player, string json)
        {
            Message.IsType<ClientMessage.Game.SendImage>(json, (data) =>
            {
                var prompt = GameData.SentPrompts[player];

                // Update GameData
                GameData.SubmitDrawing(player, data.Image, prompt);

                Console.WriteLine("Recieve image from {0} with {1} bytes for prompt {2}", player.Data.Name, data.Image.Length, prompt.GetText());

                // Tell all clients that player has submitted drawing
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data, GamePlayerAction.DrawingSubmitted));

                ResponseHandler.Register(player);

                if (ResponseHandler.AllResponded())
                    EndState();
            });
        }
    }

    public class StateAnsweringPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Answering; } }

        public StateAnsweringPhase(Settings settings) : base(settings)
        {

        }

        protected override void OnBegin()
        {
            SetCountdownTimer("Answering Timer", GameData.Settings.Drawesome.AnsweringTime, true);

            if (GameData.HasDrawings())
            {
                var drawing = GameData.GetDrawing();

                GameData.Players.ForEach(x => x.SendImage(GameData.GetDrawing()));

                // Create a list of players excluding the drawing's creator
                var players = GameData.Players.Where(x => x.Data.ID != drawing.Creator.ID).ToList();
                players.ForEach(x => ResponseHandler.AddRespondant(x));
            }
            else
            {
                EndState();
            }
        }

        public override void HandleMessage(Player player, string json)
        {
            Message.IsType<ClientMessage.Game.SubmitAnswer>(json, (data) =>
            {
                Console.WriteLine("{0} submitted '{1}'", player.Data.Name, data.Answer);

                var answer = StringFormatter.FormatAnswer(data.Answer);

                if (GameData.IsPrompt(answer))
                {
                    Console.WriteLine("Player {0}'s answer matches prompt!", player.Data.Name);
                    player.SendAnswerValidation(GameAnswerValidationResponse.MatchesPrompt);
                }
                else if (GameData.MatchesExistingAnswer(answer))
                {
                    Console.WriteLine("Player {0}'s answer matches an existing answer from another player!", player.Data.Name);
                    player.SendAnswerValidation(GameAnswerValidationResponse.AlreadyExists);
                }
                else
                {
                    player.SendAnswerValidation(GameAnswerValidationResponse.None);

                    // Add answer here
                    GameData.SubmitAnswer(new AnswerData(player.Data, answer));

                    // Tell all clients that player has submitted answer
                    GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data, GamePlayerAction.AnswerSubmitted));

                    // Register response
                    ResponseHandler.Register(player);

                    if (ResponseHandler.AllResponded())
                        EndState();
                }
            });
        }        
    }

    public class StateChoosingPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Choosing; } }

        public StateChoosingPhase(Settings settings) : base(settings)
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();

            SetCountdownTimer("Choosing Timer", GameData.Settings.Drawesome.ChoosingTime);

            GameData.SendChoicesToPlayers();

            // Handle responses from every player except the one that drew the image
            GameData.GetAnsweringPlayers().ForEach(x => ResponseHandler.AddRespondant(x));
        }

        public override void HandleMessage(Player player, string json)
        {
            var message = JsonHelper.FromJson<Message>(json);

            Message.IsType<ClientMessage.Game.SubmitChoice>(json, (data) =>
            {
                Console.WriteLine("Player {0} chose {1}", player.Data.Name, data.ChosenAnswer);
                GameData.SubmitChoice(data.ChosenAnswer, player);
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data, GamePlayerAction.ChoiceChosen));
                ResponseHandler.Register(player);

                // End state if all players have chosen
                if (ResponseHandler.AllResponded())
                    EndState();
            });

            Message.IsType<ClientMessage.Game.LikeAnswer>(json, (data) =>
            {
                // Add like
                GameData.SubmitLike(data.Answer);
            });
        }
    }

    public class StateResultsPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Results; } }

        Queue<AnswerData> ChosenAnswersQueue { get; set; }

        public StateResultsPhase(Settings settings) : base(settings)
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();

            // Sort chosen answers here...
            ChosenAnswersQueue = new Queue<AnswerData>();

            GameData.CalculateScores();

            // Queue up player answers and decoys by order of least chosen to most chosen
            var sortedAnswers = GameData.ChosenAnswers
                .Where(x => x.Type != GameAnswerType.ActualAnswer)
                .OrderBy(x => x.Choosers.Count)
                .ToList();
            sortedAnswers.ForEach(x => ChosenAnswersQueue.Enqueue(x));

            // Queue the actual answer
            var actualAnswer = GameData.ChosenAnswers.FirstOrDefault(x => x.Type == GameAnswerType.ActualAnswer);
            if (actualAnswer == null)
                actualAnswer = new AnswerData(GameData.CurrentDrawing.Prompt.GetText(), GameAnswerType.ActualAnswer);
            ChosenAnswersQueue.Enqueue(actualAnswer);

            UpdateQueue();
        }

        void UpdateQueue()
        {
            // Loop through all results until empty
            if (ChosenAnswersQueue.Count != 0)
            {
                GameData.Players.ForEach(x => ResponseHandler.AddRespondant(x));
                ShowNextResult(ChosenAnswersQueue.Count == 1);
            }
            else
            {
                EndState();
            }
        }

        void ShowNextResult(bool isFinalResult)
        {
            // Send choice to client and remove from queue
            var chosenAnswer = ChosenAnswersQueue.Dequeue();
            var message = new ServerMessage.Game.SendResult(chosenAnswer);
            GameData.Players.ForEach(x => x.SendMessage(message));

            float duration = isFinalResult ? Settings.Drawesome.TimeToShowFinalResult : Settings.Drawesome.TimeToShowResult;
            string name = isFinalResult ? "Display actual answer" : "Display player answer";
            var timer = new GameTimer(name, duration, () => UpdateQueue());
        }
    }

    public class StateScores : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Scores; } }

        public StateScores(Settings settings) : base(settings)
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();

            var score = GameData.GetLatestScores();

            foreach (var s in score)
            {
                if (s.Value.AnswerGiven != null)
                {
                    Logger.Log("Player: {0}, Score: {1}, Answer Given: {2}", s.Key.Name, s.Value.CurrentScore, s.Value.AnswerGiven);
                }
                else
                {
                    Logger.Log("Player: {0}, Score: {1}, Answer Given: None", s.Key.Name, s.Value.CurrentScore);
                }
            }

            GameData.Players.ForEach(x => x.SendScores(score));
            SetCountdownTimer("Show Scores", 10f, false);
        }
    }

    public class StateFinalScores : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.FinalScores; } }

        public StateFinalScores(Settings settings) : base(settings)
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();
            SetCountdownTimer("Show FInal Scores", 15f, false);

            // Send final scores
            GameData.Players.ForEach(x => x.SendScores(GameData.GetLatestScores(), true));
        }
    }

    public class StateGameOver : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.GameOver; } }

        public StateGameOver(Settings settings) : base(settings)
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();
            GameData.OnNewRound();
            EndState();
        }
    }
}
