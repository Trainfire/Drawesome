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
                GameData.SubmitDrawing(player, data.Image);

                // Tell all clients that player has submitted drawing
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data, GamePlayerAction.DrawingSubmitted));

                UpdateState(player);
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
                GameData.SendDrawingToPlayers();
                GameData.GetAnsweringPlayers().ForEach(x => ResponseHandler.AddRespondant(x));
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
                GameData.SubmitAnswer(player, data.Answer, () => UpdateState(player));
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

            SetCountdownTimer("Choosing Timer", GameData.Settings.Drawesome.ChoosingTime, true);

            GameData.SendChoicesToPlayers();

            // Handle responses from every player except the one that drew the image
            GameData.GetAnsweringPlayers().ForEach(x => ResponseHandler.AddRespondant(x));
        }

        public override void HandleMessage(Player player, string json)
        {
            var message = JsonHelper.FromJson<Message>(json);

            Message.IsType<ClientMessage.Game.SubmitChoice>(json, (data) =>
            {
                GameData.SubmitChoice(player, data.ChosenAnswer);
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data, GamePlayerAction.ChoiceChosen));
                UpdateState(player);
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

            ChosenAnswersQueue = GameData.GetResults();

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
                    Logger.Log("Player: {0}, Score: {1}, Answer Given: {2}, Likes: {3}", s.Key.Name, s.Value.Score, s.Value.AnswerGiven, s.Value.AnswerGiven.Likes);
                }
                else
                {
                    Logger.Log("Player: {0}, Score: {1}, Answer Given: None", s.Key.Name, s.Value.Score);
                }
            }

            GameData.Players.ForEach(x => x.SendScores(score));
            SetCountdownTimer("Show Scores", Settings.Drawesome.ScoresHoldTime, false);
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

            // Send final scores
            GameData.Players.ForEach(x => x.SendScores(GameData.GetLatestScores(), true));
        }
    }
}
