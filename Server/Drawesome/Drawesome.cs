using Server.Game;
using System.Collections.Generic;
using System;
using Protocol;
using System.Linq;

namespace Server.Drawesome
{
    public class DrawesomeGame : Game<DrawesomeGameData>
    {
        DrawesomeGameData Data { get; set; }

        protected override string Name { get { return "Drawesome"; } }

        public DrawesomeGame(Settings settings) : base(settings)
        {
            AddState(GameState.PreGame, new StatePreGame());
            AddState(GameState.RoundBegin, new StateRoundBegin());
            AddState(GameState.Drawing, new StateDrawingPhase());
            AddState(GameState.Answering, new StateAnsweringPhase());
            AddState(GameState.Choosing, new StateChoosingPhase());
            AddState(GameState.Results, new StateResultsPhase());
            AddState(GameState.Scores, new StateScores());
            AddState(GameState.GameOver, new StateRoundEnd());
        }

        public override void Start(List<Player> players)
        {
            base.Start(players);
            Data = new DrawesomeGameData(players, Settings);
            SetState(GameState.RoundBegin, Data);
        }

        public override void StartNewRound()
        {
            base.StartNewRound();
            SetState(GameState.Drawing, GameData);
        }

        public override void Restart()
        {
            base.Restart();
            SetState(GameState.PreGame, Data);
        }

        protected override void OnEndState(DrawesomeGameData gameData)
        {
            // Determine what to do when the current state ends
            switch (CurrentState.Type)
            {
                case GameState.PreGame:
                    SetState(GameState.PreGame, gameData);
                    break;

                case GameState.RoundBegin:
                    SetState(GameState.Drawing, gameData);
                    break;

                case GameState.Drawing:
                    SetState(GameState.Answering, gameData);
                    break;

                case GameState.Answering:

                    Console.WriteLine("Answers");
                    foreach (var answer in gameData.Answers)
                    {
                        Console.WriteLine("\t{0}: {1}", answer.Author.Name, answer.Answer);
                    }

                    SetState(GameState.Choosing, gameData);
                    break;

                case GameState.Choosing:

                    Console.WriteLine("Choices");
                    foreach (var choice in gameData.ChosenAnswers)
                    {
                        if (choice.Choosers.Count != 0)
                        {
                            var players = choice.Choosers
                            .Select(x => x.Name)
                            .Aggregate((current, next) => current + ", " + next);

                            Console.WriteLine("\t{0}: {1}", choice.Answer, players);
                        }
                    }

                    SetState(GameState.Results, gameData);
                    break;

                case GameState.Results:
                    gameData.OnNewRound();
                    if (GameData.HasDrawings())
                    {
                        SetState(GameState.Scores, gameData);
                    }
                    else
                    {
                        SetState(GameState.GameOver, gameData);
                    }
                    break;

                case GameState.Scores:
                    // After scores, return to Answering phase for remaining drawings
                    SetState(GameState.Answering, gameData);
                    break;

                case GameState.GameOver:
                    Log("Game Over!");
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
            SetTimer("Begin Timer", GameData.Settings.Drawesome.RoundBeginTime, true);
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
            SetTimer("Drawing Timer", GameData.Settings.Drawesome.DrawTime, true);

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
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data));

                ResponseHandler.Register(player);

                if (ResponseHandler.AllResponded())
                    EndState();
            });
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
            SetTimer("Answering Timer", GameData.Settings.Drawesome.AnsweringTime, true);

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
                    GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data));

                    // Register response
                    ResponseHandler.Register(player);

                    if (ResponseHandler.AllResponded())
                        EndState();
                }
            });
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

            SetTimer("Choosing Timer", GameData.Settings.Drawesome.ChoosingTime);

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
                    GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data));
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
            timer.Finish += Timer_Finish;
        }

        void Timer_Finish(object sender, EventArgs e)
        {
            EndState();
        }

        void ShowNextResult()
        {
            // Send choice to client and remove from queue
            var chosenAnswer = ChosenAnswersQueue.Dequeue();
            var message = new ServerMessage.Game.SendResult(chosenAnswer);
            GameData.Players.ForEach(x => x.SendMessage(message));
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
    }

    public class StateScores : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Scores; } }

        protected override void OnBegin()
        {
            base.OnBegin();
            var score = GameData.Scores.ToDictionary(x => x.Key, y => y.Value);
            GameData.Players.ForEach(x => x.SendScores(score));
            SetTimer("Show Scores", 10f, false);
        }
    }

    public class StateRoundEnd : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.GameOver; } }

        protected override void OnBegin()
        {
            base.OnBegin();
            GameData.OnNewRound();
            EndState(true);
        }
    }

    #endregion
}
