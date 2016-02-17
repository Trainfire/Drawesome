using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System.Linq;
using System;

public class Game : MonoBehaviour, IClientHandler
{
    public Timer Timer;
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
    public UiGameStateGameOver RoundEndView;

    Client Client { get; set; }
    List<IGameState> States { get; set; }
    List<IGameStateHandler> StateHandlers { get; set; }
    List<IGameMessageHandler> MessageHandlers { get; set; }
    List<IGameTransitionhandler> TransitionHandlers { get; set; }
    State Current { get; set; }
    DrawingCanvas Canvas { get; set; }
    PlayerList PlayerList { get; set; }

    public void Initialise(Client client)
    {
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
    }

    void OnMessage(string json)
    {
        MessageHandlers.ForEach(x => x.HandleMessage(json));

        // Change state
        Message.IsType<ServerMessage.Game.StateChange>(json, (data) =>
        {
            ChangeState(data.GameState);
        });

        // Transition
        Message.IsType<ServerMessage.Game.SendTransitionPeriod>(json, (data) =>
        {
            TransitionHandlers.ForEach(x => x.HandleTransition(data));
        });
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

        public PreGameState(Client client, UiGameStatePreGame view) : base(client, view)
        {
            view.Start.onClick.AddListener(() => client.Messenger.StartGame());
        }

        protected override void OnBegin()
        {
            base.OnBegin();
            Canvas.Clear();
        }

        public override void Update()
        {
            var view = GetView<UiGameStatePreGame>();
            bool isClientOwner = Client.Connection.IsRoomOwner();
            view.Start.gameObject.SetActive(isClientOwner);
            view.InfoLabel.text = isClientOwner ? Strings.StartGame : string.Format(Strings.WaitingForRoomOwner, Client.Connection.Room.Owner.Name);
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
            view.InfoBox.Label.text = Strings.DrawingSubmitted;

            // Send image on submit
            view.Submit.onClick.AddListener(() =>
            {
                Client.Messenger.SendImage(Canvas.GetEncodedImage());
                Canvas.AllowDrawing = false;
                view.InfoBox.Show();
                view.Submit.gameObject.SetActive(false);
            });
        }

        protected override void OnBegin()
        {
            GetView<UiGameStateDrawing>((view) =>
            {
                view.InfoBox.Hide();
                view.Submit.gameObject.SetActive(true);
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
            view.Submit.onClick.AddListener(() =>
            {
                Client.Messenger.SubmitAnswer(view.InputField.text);
            });
        }

        protected override void OnBegin()
        {
            GetView<UiGameStateAnswering>((view) =>
            {
                view.InfoBox.Hide();
                view.InputField.text = Strings.PromptEnterGuess;
                view.InputField.gameObject.SetActive(true);
                view.Submit.gameObject.SetActive(true);
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
            view.OnChoiceSelected += (choice) =>
            {
                Client.Messenger.SubmitChosenAnswer(choice.Answer);
            };

            view.OnLike += (choice) =>
            {
                Client.Messenger.SubmitLike(choice.Answer);
            };
        }

        protected override void OnBegin()
        {
            base.OnBegin();
            GetView<UiGameStateChoosing>().InfoBox.Hide();
        }

        protected override void OnMessage(string json)
        {
            Message.IsType<ServerMessage.Game.SendChoices>(json, (data) =>
            {
                GetView<UiGameStateChoosing>((view) =>
                {
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
            view.OnFinishedShowingResult += () =>
            {
                client.Messenger.FinishShowingResult();
            };
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

        public ScoresState(Client client, UiGameStateScores view) : base(client, view)
        {

        }

        protected override void OnMessage(string json)
        {
            Message.IsType<ServerMessage.Game.SendScores>(json, (data) =>
            {
                // Order scores
                var sortedList = new List<KeyValuePair<PlayerData, uint>>();
                for (int i = 0; i < data.Players.Count; i++)
                {
                    sortedList.Add(new KeyValuePair<PlayerData, uint>(data.Players[i], data.Scores[i]));
                }
                sortedList = sortedList.OrderByDescending(x => x.Value).ToList();

                //GetView<UiGameStateScores>().ShowScores(sortedList);
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
