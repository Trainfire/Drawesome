using UnityEngine;
using System.Collections.Generic;
using Protocol;

public class Game : MonoBehaviour, IClientHandler
{
    public Timer Timer;
    public UiGameStateRoundBegin RoundBeginView;
    public UiGameStateDrawing DrawingView;
    public UiGameStateAnswering AnsweringView;
    public UiGameStateChoosing ChoosingView;
    public UiGameStateResults ResultsView;
    public UiGameStateRoundEnd RoundEndView;

    Client Client { get; set; }
    List<State> States { get; set; }

    public void Initialise(Client client)
    {
        Client = client;
        Client.MessageHandler.OnMessage += OnMessage;

        AddState(new RoundBeginState(client, RoundBeginView));
        AddState(new DrawingState(client, DrawingView));
        AddState(new AnsweringState(client, AnsweringView));
        AddState(new ChoosingState(client, ChoosingView));
        AddState(new ResultsState(client, ResultsView));
        AddState(new RoundEndState(client, RoundEndView));

        ChangeState(GameState.RoundBegin);
    }

    void AddState(State state)
    {
        if (States == null)
            States = new List<State>();

        States.Add(state);
    }

    void ChangeState(GameState nextState)
    {
        foreach (var s in States)
        {
            if (s.Type != nextState)
            {
                s.Hide();
            }
            else
            {
                s.Show();
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

    public abstract class State
    {
        public abstract GameState Type { get; }
        protected UiGameState View { get; private set; }
        protected Client Client { get; private set; }
        private bool IsFirstShow { get; set; }

        public State(Client client, UiGameState view)
        {
            Client = client;
            Client.MessageHandler.OnMessage += OnMessage;
            View = view;
            IsFirstShow = true;
        }

        public void Show()
        {
            View.gameObject.SetActive(true);

            if (IsFirstShow)
            {
                OnFirstShow();
                IsFirstShow = false;
            }

            OnShow();
        }

        public void Hide()
        {
            View.gameObject.SetActive(false);
            OnHide();
        }

        protected T GetView<T>() where T : UiGameState
        {
            return View as T;
        }

        protected virtual void OnFirstShow()
        {       
            
        }

        protected virtual void OnShow()
        {

        }

        protected virtual void OnMessage(string json)
        {

        }

        protected virtual void OnHide()
        {
            
        }
    }

    public class RoundBeginState : State
    {
        public override GameState Type { get { return GameState.RoundBegin; } }

        public RoundBeginState(Client client, UiGameStateRoundBegin view) : base(client, view)
        {
            
        }

        protected override void OnMessage(string json)
        {
            // TODO: Show player's readying up here
            
        }
    }

    public class DrawingState : State
    {
        public override GameState Type { get { return GameState.Drawing; } }

        public DrawingState(Client client, UiGameStateDrawing view) : base(client, view)
        {
            // Set brush color
            view.Canvas.SetBrushColor(Client.Connection.Data.RoomId);

            // Send image on submit
            view.Submit.onClick.AddListener(() =>
            {
                Client.Messenger.SendImage(view.Canvas.GetEncodedImage);
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

    public class AnsweringState : State
    {
        public override GameState Type { get { return GameState.Answering; } }

        public AnsweringState(Client client, UiGameStateAnswering view) : base(client, view)
        {
            view.Submit.onClick.AddListener(() =>
            {
                Client.Messenger.SubmitAnswer(view.InputField.text);
            });
        }

        protected override void OnShow()
        {
            GetView<UiGameStateAnswering>().Error.Hide();
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

    public class ChoosingState : State
    {
        public override GameState Type { get { return GameState.Choosing; } }

        public ChoosingState(Client client, UiGameStateChoosing view) : base(client, view)
        {
            view.OnChoiceSelected += View_OnChoiceSelected;
        }

        void View_OnChoiceSelected(UiChoosingItem obj)
        {
            Client.Messenger.SubmitChoice(obj.Text.text);
        }

        protected override void OnMessage(string json)
        {
            Message.IsType<ServerMessage.Game.SendChoices>(json, (data) =>
            {
                GetView<UiGameStateChoosing>().ShowChoices(data.Choices);
            });
        }
    }

    public class ResultsState : State
    {
        public override GameState Type { get { return GameState.Results; } }

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

    public class RoundEndState : State
    {
        public override GameState Type { get { return GameState.RoundEnd; } }

        public RoundEndState(Client client, UiGameStateRoundEnd view) : base(client, view)
        {

        }
    }
}
