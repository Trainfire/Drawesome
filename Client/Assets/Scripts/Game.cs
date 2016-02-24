using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System.Linq;
using System;
using System.Collections;

public class Game : MonoBehaviour, IClientHandler
{
    public Timer Timer;
    public Transition Transition;
    public UiGame View;
    public UiPlayerList PlayerListView;
    public UiDrawingCanvas CanvasView;
    public UiGameStatePreGame PreGameView;
    public UiGameStateRoundBegin RoundBeginView;
    public UiGameStateDrawing DrawingView;
    public UiGameStateAnswering AnsweringView;
    public UiGameStateChoosing ChoosingView;
    public UiGameStateResults ResultsView;
    public UiGameStateScores ScoresView;
    public UiGameStateFinalScores FinalScoresView;
    public UiGameStateGameOver RoundEndView;

    Client Client { get; set; }
    List<IGameState> States { get; set; }
    List<IGameStateHandler> StateHandlers { get; set; }
    List<IGameMessageHandler> MessageHandlers { get; set; }
    List<IGameTransitionhandler> TransitionHandlers { get; set; }
    List<IGameStateEndHandler> StateEndHandlers { get; set; }
    State Current { get; set; }
    DrawingCanvas Canvas { get; set; }
    PlayerList PlayerList { get; set; }
    GameState CurrentState { get; set; }

    public void Initialise(Client client)
    {
        Debug.Log("Initialise");

        Client = client;
        Client.MessageHandler.OnMessage += OnMessage;

        // Game state logic and views
        AddState(new PreGameState(client, PreGameView));
        AddState(new RoundBeginState(client, RoundBeginView));
        AddState(new DrawingState(client, DrawingView));
        AddState(new AnsweringState(client, AnsweringView));
        AddState(new ChoosingState(client, ChoosingView));
        AddState(new ResultsState(client, ResultsView));
        AddState(new ScoresState(client, ScoresView));
        AddState(new FinalScoresState(client, FinalScoresView));
        AddState(new GameOverState(client, RoundEndView));

        // Drawing canvas
        Canvas = new DrawingCanvas(client, CanvasView);
        AddStateHandler(Canvas);

        // Player List that appears on the right
        PlayerList = new PlayerList(client, PlayerListView);
        AddMessageHandler(PlayerList);
        AddStateHandler(PlayerList);

        // Game View
        AddStateHandler(View);
        AddTransitionHandler(View);

        // Timer
        AddMessageHandler(Timer);
        AddStateHandler(Timer);

        // Transition Message
        AddStateEndHandler(Transition);
        AddStateHandler(Transition);

        ChangeState(GameState.PreGame);
    }

    void AddState(IGameState state)
    {
        if (States == null)
            States = new List<IGameState>();

        States.Add(state);
    }

    void AddStateHandler(IGameStateHandler handler)
    {
        if (StateHandlers == null)
            StateHandlers = new List<IGameStateHandler>();

        StateHandlers.Add(handler);
    }

    void AddStateEndHandler(IGameStateEndHandler handler)
    {
        if (StateEndHandlers == null)
            StateEndHandlers = new List<IGameStateEndHandler>();

        StateEndHandlers.Add(handler);
    }

    void AddMessageHandler(IGameMessageHandler handler)
    {
        if (MessageHandlers == null)
            MessageHandlers = new List<IGameMessageHandler>();

        MessageHandlers.Add(handler);
    }

    void AddTransitionHandler(IGameTransitionhandler handler)
    {
        if (TransitionHandlers == null)
            TransitionHandlers = new List<IGameTransitionhandler>();

        TransitionHandlers.Add(handler);
    }

    void ChangeState(GameState nextState)
    {    
        foreach (var s in States)
        {
            if (s.Type != nextState)
            {
                s.State.End();
            }
            else
            {
                Current = s.State;
                s.Canvas = Canvas;
                s.State.Begin();
            }
        }

        StateHandlers.ForEach(x => x.HandleState(nextState));

        CurrentState = nextState;
    }

    void OnMessage(string json)
    {
        MessageHandlers.ForEach(x => x.HandleMessage(json));

        // Change state
        Message.IsType<ServerMessage.Game.ChangeState>(json, (data) =>
        {
            Debug.LogFormat("Change state to {0}", data.GameState);
            ChangeState(data.GameState);
        });

        // End state
        Message.IsType<ServerMessage.Game.EndState>(json, (data) =>
        {
            Debug.LogFormat("End current state. (Reason: {0})", data.Reason);
            foreach (var handler in StateEndHandlers)
            {
                handler.HandleStateEnd(CurrentState, data.Reason);
            }
        });

        // Transition
        Message.IsType<ServerMessage.Game.SendTransitionPeriod>(json, (data) =>
        {
            TransitionHandlers.ForEach(x => x.HandleTransition(data));
        });
    }

    void OnDestroy()
    {
        if (Current != null)
            Current.End();

        if (Client != null)
            Client.MessageHandler.OnMessage -= OnMessage;
    }

    void Update()
    {
        if (Current != null)
            Current.Update();
    }

    public interface IGameState
    {
        State State { get; }
        GameState Type { get; }
        DrawingCanvas Canvas { set; }
    }

    public interface IGameStateHandler
    {
        void HandleState(GameState state);
    }

    public interface IGameStateEndHandler
    {
        void HandleStateEnd(GameState currentState, GameStateEndReason reason);
    }

    public interface IGameMessageHandler
    {
        void HandleMessage(string json);
    }

    public interface IGameTransitionhandler
    {
        // Hmm?
        void HandleTransition(ServerMessage.Game.SendTransitionPeriod transition);
    }

    public class PreGameState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.PreGame; } }
        public DrawingCanvas Canvas { private get; set; }

        bool IsCountingDown { get; set; }

        public PreGameState(Client client, UiGameStatePreGame view) : base(client, view)
        {
            
        }

        protected override void OnBegin()
        {
            base.OnBegin();
            Canvas.Clear();

            GetView<UiGameStatePreGame>((view) =>
            {
                view.Start.onClick.AddListener(() => Client.Messenger.StartGame());
                view.Cancel.onClick.AddListener(() => Client.Messenger.CancelGameStart());
                view.Start.gameObject.SetActive(false);
                view.Cancel.gameObject.SetActive(false);
            });
        }

        public override void Update()
        {
            var view = GetView<UiGameStatePreGame>();

            // Show info box "Waiting for Room Owner" if player is NOT the room owner
            view.InfoLabel.text = Client.Connection.IsRoomOwner() ? Strings.StartGame : string.Format(Strings.WaitingForRoomOwner, Client.Connection.Room.Owner.Name);

            // Show Start button if room owner
            view.Start.gameObject.SetActive(Client.Connection.IsRoomOwner() && !Client.Connection.Room.GameStarted);

            // Enable info box if game hasn't started, disable it if it has
            view.InfoBox.SetActive(!Client.Connection.Room.GameStarted);
        }

        protected override void OnMessage(string json)
        {
            var view = GetView<UiGameStatePreGame>();

            // Hide Start button, show Cancel button if room owner
            Message.IsType<ServerMessage.NotifyRoomCountdown>(json, (data) =>
            {
                if (Client.Connection.IsRoomOwner())
                {
                    view.Start.gameObject.SetActive(false);
                    view.Cancel.gameObject.SetActive(true);
                }

                view.SetCountdown(data.Duration);
            });

            // Show Start button, hide Cancel button if room owner
            Message.IsType<ServerMessage.NotifyRoomCountdownCancel>(json, (data) =>
            {
                if (Client.Connection.IsRoomOwner())
                {
                    view.Start.gameObject.SetActive(true);
                    view.Cancel.gameObject.SetActive(false);
                }

                view.CancelCountdown();
            });
        }

        protected override void OnEnd()
        {
            GetView<UiGameStatePreGame>((view) =>
            {
                view.Start.onClick.RemoveAllListeners();
                view.Cancel.onClick.RemoveAllListeners();
            });
        }
    }

    public class RoundBeginState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.RoundBegin; } }
        public DrawingCanvas Canvas { private get; set; }

        public RoundBeginState(Client client, UiGameStateRoundBegin view) : base(client, view)
        {
            
        }

        protected override void OnMessage(string json)
        {
            // TODO: Show player's readying up here
            
        }
    }

    public class DrawingState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.Drawing; } }
        public DrawingCanvas Canvas { private get; set; }

        public DrawingState(Client client, UiGameStateDrawing view) : base(client, view)
        {
            
        }

        protected override void OnBegin()
        {
            GetView<UiGameStateDrawing>((view) =>
            {
                view.InfoBox.Label.text = Strings.DrawingSubmitted;
                view.InfoBox.Hide();
                view.Submit.gameObject.SetActive(true);

                view.Submit.onClick.AddListener(() =>
                {
                    Client.Messenger.SendImage(Canvas.GetEncodedImage());
                    Canvas.AllowDrawing = false;
                    view.InfoBox.Show();
                    view.Submit.gameObject.SetActive(false);
                });
            });
        }

        protected override void OnMessage(string json)
        {
            Message.IsType<ServerMessage.Game.SendPrompt>(json, (data) =>
            {
                GetView<UiGameStateDrawing>().SetPrompt(data.Prompt);
            });
        }
    }

    public class AnsweringState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.Answering; } }
        public DrawingCanvas Canvas { private get; set; }

        public AnsweringState(Client client, UiGameStateAnswering view) : base(client, view)
        {

        }

        protected override void OnBegin()
        {
            GetView<UiGameStateAnswering>((view) =>
            {
                view.InfoBox.Hide();
                view.InputField.text = Strings.PromptEnterGuess;
                view.InputField.gameObject.SetActive(true);
                view.Submit.gameObject.SetActive(true);
                view.Submit.onClick.AddListener(() =>
                {
                    Client.Messenger.SubmitAnswer(view.InputField.text);
                });
            });
        }

        protected override void OnMessage(string json)
        {
            var view = GetView<UiGameStateAnswering>();

            Message.IsType<ServerMessage.Game.SendImage>(json, (data) =>
            {
                Canvas.SetImage(data.Drawing);
                bool isClientsDrawing = Client.IsPlayer(data.Drawing.Creator);
                view.InputField.gameObject.SetActive(!isClientsDrawing);
                view.Submit.gameObject.SetActive(!isClientsDrawing);

                if (isClientsDrawing)
                    view.InfoBox.Show(Strings.PlayersOwnDrawing);
            });

            Message.IsType<ServerMessage.Game.SendAnswerValidation>(json, (data) =>
            {
                view.InfoBox.Show(data.Response);
                if (data.Response == GameAnswerValidationResponse.None)
                {
                    OnAnswerValidation();
                }
                else
                {
                    view.InputField.text = string.Empty;
                }
            });
        }

        void OnAnswerValidation()
        {
            GetView<UiGameStateAnswering>((view) =>
            {
                view.InputField.gameObject.SetActive(false);
                view.Submit.gameObject.SetActive(false);
                view.InfoBox.Show(Strings.AnswerSubmitted);
            });   
        }
    }

    public class ChoosingState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.Choosing; } }
        public DrawingCanvas Canvas { private get; set; }

        public ChoosingState(Client client, UiGameStateChoosing view) : base(client, view)
        {
            
        }

        protected override void OnBegin()
        {
            base.OnBegin();
            GetView<UiGameStateChoosing>((view) =>
            {
                view.InfoBox.Hide();

                view.OnChoiceSelected += (choice) =>
                {
                    Client.Messenger.SubmitChosenAnswer(choice.Answer);
                };

                view.OnLike += (choice) =>
                {
                    Client.Messenger.SubmitLike(choice.Answer);
                };
            });
        }

        protected override void OnMessage(string json)
        {
            Message.IsType<ServerMessage.Game.SendChoices>(json, (data) =>
            {
                GetView<UiGameStateChoosing>((view) =>
                {
                    // Randomise order of choices before showing on UI
                    data.Choices.Shuffle();

                    // Send to UI
                    view.ShowChoices(data.Creator, data.Choices);
                });
            });
        }
    }

    public class ResultsState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.Results; } }
        public DrawingCanvas Canvas { private get; set; }

        public ResultsState(Client client, UiGameStateResults view) : base(client, view)
        {
            
        }

        protected override void OnBegin()
        {
            base.OnBegin();

            GetView<UiGameStateResults>((view) =>
            {
                view.OnFinishedShowingResult += () =>
                {
                    Client.Messenger.FinishShowingResult();
                };
            });
        }

        protected override void OnMessage(string json)
        {
            Message.IsType<ServerMessage.Game.SendResult>(json, (data) =>
            {
                GetView<UiGameStateResults>().ShowAnswer(data.Answer);
            });
        }
    }

    public class ScoresState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.Scores; } }
        public DrawingCanvas Canvas { private get; set; }

        Dictionary<PlayerData, GameScore> scoreCache = new Dictionary<PlayerData, GameScore>();

        public ScoresState(Client client, UiGameStateScores view) : base(client, view)
        {

        }

        protected override void OnMessage(string json)
        {
            Message.IsType<ServerMessage.Game.SendScores>(json, (data) =>
            {
                // Make dictionary mapping players[i] to scores[i]
                var scores = Enumerable.Range(0, data.Players.Count)
                .ToDictionary(i => data.Players[i], i => data.Scores[i]);

                foreach (var score in scores)
                {
                    if (!scoreCache.ContainsKey(score.Key))
                        scoreCache.Add(score.Key, new GameScore(score.Value));

                    // Cache previous score
                    scoreCache[score.Key].PreviousScore = scoreCache[score.Key].CurrentScore;

                    Debug.LogFormat("{0}'s previous score is {1}", score.Key.Name, scoreCache[score.Key].PreviousScore);

                    // Set current score
                    scoreCache[score.Key].CurrentScore = score.Value.CurrentScore;
                }

                GetView<UiGameStateScores>().ShowScores(scoreCache);
            });
        }
    }

    public class FinalScoresState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.FinalScores; } }
        public DrawingCanvas Canvas { private get; set; }

        public FinalScoresState(Client client, UiGameStateFinalScores view) : base(client, view)
        {

        }

        protected override void OnMessage(string json)
        {
            Message.IsType<ServerMessage.Game.SendScores>(json, (data) =>
            {
                // Make dictionary mapping players to scores.
                var scores = Enumerable.Range(0, data.Players.Count)
                .ToDictionary(i => data.Players[i], i => data.Scores[i]);

                GetView<UiGameStateFinalScores>().Show(scores);
            });
        }
    }

    public class GameOverState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.GameOver; } }
        public DrawingCanvas Canvas { private get; set; }

        public GameOverState(Client client, UiGameStateGameOver view) : base(client, view)
        {

        }
    }
}
