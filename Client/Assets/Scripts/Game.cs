using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System.Linq;

public class Game : MonoBehaviour, IClientHandler
{
    public Timer Timer;
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
    State Current { get; set; }

    public void Initialise(Client client)
    {
        Client = client;
        Client.MessageHandler.OnMessage += OnMessage;

        AddState(new PreGameState(client, PreGameView));
        AddState(new RoundBeginState(client, RoundBeginView));
        AddState(new DrawingState(client, DrawingView));
        AddState(new AnsweringState(client, AnsweringView));
        AddState(new ChoosingState(client, ChoosingView));
        AddState(new ResultsState(client, ResultsView));
        AddState(new ScoresState(client, ScoresView));
        AddState(new GameOverState(client, RoundEndView));

        ChangeState(GameState.PreGame);
    }

    void AddState(IGameState state)
    {
        if (States == null)
            States = new List<IGameState>();

        States.Add(state);
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
                s.State.Begin();
            }
        }
    }

    void OnMessage(string json)
    {
        // Change state
        Message.IsType<ServerMessage.Game.StateChange>(json, (data) =>
        {
            ChangeState(data.GameState);
        });

        // Show and set timer
        Message.IsType<ServerMessage.Game.AddTimer>(json, (data) =>
        {
            Timer.Show();
            Timer.SetDuration(data.Duration);
        });

        // Update timer
        Message.IsType<ServerMessage.Game.SetTimer>(json, (data) =>
        {
            Timer.SetTime(data.CurrentTime);
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
    }

    public class PreGameState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.PreGame; } }

        public PreGameState(Client client, UiGameStatePreGame view) : base(client, view)
        {
            view.Start.onClick.AddListener(() => client.Messenger.StartGame());
        }

        public override void Update()
        {
            var view = GetView<UiGameStatePreGame>();
            bool isClientOwner = Client.Connection.IsRoomOwner();
            view.InfoBox.SetActive(!isClientOwner);
            view.Start.interactable = isClientOwner;
            view.InfoLabel.text = string.Format("Waiting for room owner: {0}", Client.Connection.Room.Owner.Name);
        }
    }

    public class RoundBeginState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.RoundBegin; } }

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

        public DrawingState(Client client, UiGameStateDrawing view) : base(client, view)
        {
            // Send image on submit
            view.Submit.onClick.AddListener(() =>
            {
                Client.Messenger.SendImage(view.Canvas.GetEncodedImage);
            });
        }

        protected override void OnBegin()
        {
            base.OnFirstBegin();

            // Set brush color
            GetView<UiGameStateDrawing>().Canvas.SetBrushColor(Client.Connection.Player.RoomId);
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

        public AnsweringState(Client client, UiGameStateAnswering view) : base(client, view)
        {
            view.Submit.onClick.AddListener(() =>
            {
                Client.Messenger.SubmitAnswer(view.InputField.text);
                view.InputField.interactable = false;
                view.InputField.text = "Answer submitted! Waiting for other players...";
                view.Submit.interactable = false;
            });
        }

        protected override void OnBegin()
        {
            GetView<UiGameStateAnswering>((view) =>
            {
                view.Error.Hide();
                view.InputField.text = "Enter your guess here...";
                view.InputField.interactable = true;
                view.Submit.interactable = true;
            });
        }

        protected override void OnMessage(string json)
        {
            Message.IsType<ServerMessage.Game.SendImage>(json, (data) =>
            {
                GetView<UiGameStateAnswering>().Canvas.SetImage(data.Image);
            });

            Message.IsType<ServerMessage.Game.SendAnswerValidation>(json, (data) =>
            {
                var errorView = GetView<UiGameStateAnswering>().Error;
                errorView.Show(data.Error);
            });
        }
    }

    public class ChoosingState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.Choosing; } }

        public ChoosingState(Client client, UiGameStateChoosing view) : base(client, view)
        {
            view.OnChoiceSelected += View_OnChoiceSelected;
        }

        void View_OnChoiceSelected(UiChoosingItem obj)
        {
            Client.Messenger.SubmitChosenAnswer(obj.Text[0].text);
        }

        protected override void OnMessage(string json)
        {
            Message.IsType<ServerMessage.Game.SendChoices>(json, (data) =>
            {
                GetView<UiGameStateChoosing>().ShowChoices(data.Choices);
            });
        }
    }

    public class ResultsState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.Results; } }

        public ResultsState(Client client, UiGameStateResults view) : base(client, view)
        {
            
        }

        protected override void OnMessage(string json)
        {
            Message.IsType<ServerMessage.Game.SendResult>(json, (data) =>
            {
                GetView<UiGameStateResults>().ShowResult(data.Result);
            });
        }
    }

    public class ScoresState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.Scores; } }

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

                GetView<UiGameStateScores>().ShowScores(sortedList);
            });
        }
    }

    public class GameOverState : State, IGameState
    {
        public State State { get { return this; } }
        public GameState Type { get { return GameState.GameOver; } }

        public GameOverState(Client client, UiGameStateGameOver view) : base(client, view)
        {

        }
    }
}
