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
    }

    public class StateRoundBegin : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.RoundBegin; } }

        protected override void OnBegin()
        {
            base.OnBegin();
            SetCountdownTimer("Begin Timer", GameData.Settings.Drawesome.RoundBeginTime, true);
        }

        protected override void OnCountdownFinish(object sender, EventArgs e)
        {
            EndState(GameData.Settings.Drawesome.Transitions.RoundBeginToDrawing, GameTransition.RoundBeginToDrawing);
        }
    }

    public class StateDrawingPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Drawing; } }

        ResponseHandler<Player> ResponseHandler { get; set; }

        public StateDrawingPhase()
        {
            ResponseHandler = new ResponseHandler<Player>();
        }

        protected override void OnBegin()
        {
            SetCountdownTimer("Drawing Timer", GameData.Settings.Drawesome.DrawTime, true);

            ResponseHandler.Clear();

            // Send random prompts to players
            foreach (var player in GameData.Players)
            {
                ResponseHandler.AddRespondant(player);
                player.SendPrompt(GameData.GetPrompt(player).Text);
            }
        }

        public override void OnPlayerMessage(Player player, string json)
        {
            Message.IsType<ClientMessage.Game.SendImage>(json, (data) =>
            {
                var prompt = GameData.SentPrompts[player];

                // Update GameData
                if (!GameData.Drawings.Any(x => x.Creator.ID == player.Data.ID))
                    GameData.AddDrawing(new DrawingData(player.Data, data.Image, prompt));

                Console.WriteLine("Recieve image from {0} with {1} bytes for prompt {2}", player.Data.Name, data.Image.Length, prompt.Text);

                // Tell all clients that player has submitted drawing
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data, GamePlayerAction.DrawingSubmitted));

                ResponseHandler.Register(player);

                if (ResponseHandler.AllResponded())
                    EndState(GameData.Settings.Drawesome.Transitions.DrawingToAnswering, GameTransition.DrawingToAnswering);
            });
        }

        protected override void OnCountdownFinish(object sender, EventArgs e)
        {
            EndState(GameData.Settings.Drawesome.Transitions.DrawingToAnswering, GameTransition.DrawingToAnswering, true);
        }
    }

    public class StateAnsweringPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Answering; } }

        ResponseHandler<Player> ResponseHandler { get; set; }

        public StateAnsweringPhase()
        {
            ResponseHandler = new ResponseHandler<Player>();
        }

        protected override void OnBegin()
        {
            SetCountdownTimer("Answering Timer", GameData.Settings.Drawesome.AnsweringTime, true);

            if (GameData.HasDrawings())
            {
                var drawing = GameData.GetDrawing();

                GameData.Players.ForEach(x => x.SendImage(GameData.GetDrawing()));

                ResponseHandler.Clear();

                // Create a list of players excluding the drawing's creator
                var players = GameData.Players.Where(x => x.Data.ID != drawing.Creator.ID).ToList();
                players.ForEach(x => ResponseHandler.AddRespondant(x));
            }
            else
            {
                EndState();
            }
        }

        public override void OnPlayerMessage(Player player, string json)
        {
            Message.IsType<ClientMessage.Game.SubmitAnswer>(json, (data) =>
            {
                Console.WriteLine("{0} submitted '{1}'", player.Data.Name, data.Answer);

                if (IsPrompt(data.Answer))
                {
                    Console.WriteLine("Player {0}'s answer matches prompt!", player.Data.Name);
                    player.SendAnswerValidation(GameAnswerValidationResponse.MatchesPrompt);
                }
                else if (HasPromptBeenSubmitted(data.Answer))
                {
                    Console.WriteLine("Player {0}'s answer matches an existing answer from another player!", player.Data.Name);
                    player.SendAnswerValidation(GameAnswerValidationResponse.AlreadyExists);
                }
                else
                {
                    player.SendAnswerValidation(GameAnswerValidationResponse.None);

                    // Add answer here
                    GameData.AddAnswer(new AnswerData(player.Data, data.Answer));

                    // Tell all clients that player has submitted answer
                    GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data, GamePlayerAction.AnswerSubmitted));

                    // Register response
                    ResponseHandler.Register(player);

                    if (ResponseHandler.AllResponded())
                        EndState();
                }
            });
        }

        protected override void OnCountdownFinish(object sender, EventArgs e)
        {
            EndState(GameData.Settings.Drawesome.Transitions.AnsweringToChoosing, GameTransition.AnsweringToChoosing, true);
        }

        protected override void OnEndState()
        {
            GameData.AddDecoys();
            GameData.AddActualAnswer();
            base.OnEndState();
        }

        bool IsPrompt(string answer)
        {
            return GameData.SentPrompts.Any(x => x.Value.Text.ToLower() == answer.ToLower());
        }

        bool HasPromptBeenSubmitted(string answer)
        {
            return GameData.Answers.Any(x => x.Answer.ToLower() == answer.ToLower());
        }
    }

    public class StateChoosingPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Choosing; } }

        ResponseHandler<Player> ResponseHandler { get; set; }

        public StateChoosingPhase()
        {
            ResponseHandler = new ResponseHandler<Player>();
        }

        protected override void OnBegin()
        {
            base.OnBegin();

            SetCountdownTimer("Choosing Timer", GameData.Settings.Drawesome.ChoosingTime);

            GameData.Players.ForEach(x => x.SendChoices(GameData.CurrentDrawing.Creator, GameData.Answers.ToList()));

            // Handle responses from every player except the one that drew the image
            ResponseHandler.Clear();
            var players = GameData.Players.Where(x => x.Data.ID != GameData.CurrentDrawing.Creator.ID).ToList();
            players.ForEach(x => ResponseHandler.AddRespondant(x));
        }

        public override void OnPlayerMessage(Player player, string json)
        {
            var message = JsonHelper.FromJson<Message>(json);

            Message.IsType<ClientMessage.Game.SubmitChoice>(json, (data) =>
            {
                if (!IsPlayersOwnAnswer(player))
                {
                    GameData.AddChosenAnswer(data.ChosenAnswer, player);
                    GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data, GamePlayerAction.ChoiceChosen));
                    ResponseHandler.Register(player);

                    // End state if all players have chosen
                    if (ResponseHandler.AllResponded())
                        EndState();
                }
            });

            Message.IsType<ClientMessage.Game.LikeAnswer>(json, (data) =>
            {
                // Add like
                GameData.AddLike(data.Answer);
            });
        }

        bool IsPlayersOwnAnswer(Player player)
        {
            return GameData.ChosenAnswers.Any(x => x.Author.ID == player.Data.ID);
        }

        protected override void OnCountdownFinish(object sender, EventArgs e)
        {
            EndState(GameData.Settings.Drawesome.Transitions.ChoosingtoResults, GameTransition.ChoosingToResults, true);
        }
    }

    public class StateResultsPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Results; } }

        Queue<AnswerData> ChosenAnswersQueue { get; set; }
        ResponseHandler<Player> ResponseHandler { get; set; }

        public StateResultsPhase()
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();

            ResponseHandler = new ResponseHandler<Player>();

            // Sort chosen answers here...
            ChosenAnswersQueue = new Queue<AnswerData>();

            // Queue up player answers and decoys by order of least chosen to most chosen
            var sortedAnswers = GameData.ChosenAnswers
                .Where(x => x.Type != GameAnswerType.ActualAnswer)
                .OrderBy(x => x.Choosers.Count)
                .ToList();
            sortedAnswers.ForEach(x => ChosenAnswersQueue.Enqueue(x));

            // Queue the actual answer
            var actualAnswer = GameData.ChosenAnswers.FirstOrDefault(x => x.Type == GameAnswerType.ActualAnswer);
            if (actualAnswer == null)
                actualAnswer = new AnswerData(GameData.CurrentDrawing.Prompt.Text, GameAnswerType.ActualAnswer);
            ChosenAnswersQueue.Enqueue(actualAnswer);

            UpdateQueue();
        }

        public override void OnPlayerMessage(Player player, string json)
        {
            Message.IsType<ClientMessage.Game.SendAction>(json, (data) =>
            {
                if (data.Action == GameAction.FinishShowingResult)
                {
                    ResponseHandler.Register(player);

                    Console.WriteLine("Have all responded? {0}", ResponseHandler.AllResponded());

                    if (ResponseHandler.AllResponded())
                        UpdateQueue();
                }
            });
        }

        void UpdateQueue()
        {
            ResponseHandler.Clear();

            if (ChosenAnswersQueue.Count != 0)
            {
                GameData.Players.ForEach(x => ResponseHandler.AddRespondant(x));
                ShowNextResult();
            }
            else
            {
                OnQueueEmpty();
            }
        }

        void OnQueueEmpty()
        {
            // Add delay before next state
            var timer = new GameTimer(GameData.Settings.Drawesome.ResultTimeBetween);
            timer.Finish += (sender, args) =>
            {
                EndState();
            };
        }

        void ShowNextResult()
        {
            // Send choice to client and remove from queue
            var chosenAnswer = ChosenAnswersQueue.Dequeue();
            var message = new ServerMessage.Game.SendResult(chosenAnswer);
            GameData.Players.ForEach(x => x.SendMessage(message));
        }

        protected override void OnCountdownFinish(object sender, EventArgs e)
        {
            EndState(GameData.Settings.Drawesome.Transitions.ResultsToScores, GameTransition.ResultsToScores, true);
        }
    }

    public class StateScores : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Scores; } }

        protected override void OnBegin()
        {
            base.OnBegin();
            var score = GameData.Scores.ToDictionary(x => x.Key, y => y.Value);
            GameData.Players.ForEach(x => x.SendScores(score));
            SetCountdownTimer("Show Scores", 10f, false);
        }
    }

    public class StateRoundEnd : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.GameOver; } }

        protected override void OnBegin()
        {
            base.OnBegin();
            GameData.OnNewRound();
            EndState(GameData.Settings.Drawesome.Transitions.ScoresToAnswering, GameTransition.ScoresToAnswering);
        }
    }
}
