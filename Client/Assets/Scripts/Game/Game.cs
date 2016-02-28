using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System;

public class Game : MonoBehaviour, IClientHandler
{
    public event EventHandler OnGameEnd;

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
    State Current { get; set; }
    DrawingCanvas Canvas { get; set; }
    PlayerList PlayerList { get; set; }
    GameState CurrentState { get; set; }

    List<IGameState> States = new List<IGameState>();
    List<IGameStateHandler> StateHandlers = new List<IGameStateHandler>();
    List<IGameMessageHandler> MessageHandlers = new List<IGameMessageHandler>();
    List<IGameTransitionhandler> TransitionHandlers = new List<IGameTransitionhandler>();
    List<IGameStateEndHandler> StateEndHandlers = new List<IGameStateEndHandler>();

    public void Initialise(Client client)
    {
        Debug.Log("Initialise");

        Client = client;
        Client.MessageHandler.OnMessage += OnMessage;

        // Game state logic and views
        States.Add(new PreGameState(client, PreGameView));
        States.Add(new RoundBeginState(client, RoundBeginView));
        States.Add(new DrawingState(client, DrawingView));
        States.Add(new AnsweringState(client, AnsweringView));
        States.Add(new ChoosingState(client, ChoosingView));
        States.Add(new ResultsState(client, ResultsView));
        States.Add(new ScoresState(client, ScoresView));
        States.Add(new FinalScoresState(client, FinalScoresView));
        States.Add(new GameOverState(client, RoundEndView));

        // Drawing canvas
        Canvas = new DrawingCanvas(client, CanvasView);
        StateHandlers.Add(Canvas);

        // Player List that appears on the right
        PlayerList = new PlayerList(client, PlayerListView);
        MessageHandlers.Add(PlayerList);
        StateHandlers.Add(PlayerList);

        // Game View
        StateHandlers.Add(View);
        TransitionHandlers.Add(View);

        // Timer
        MessageHandlers.Add(Timer);
        StateHandlers.Add(Timer);

        // Transition Message
        StateEndHandlers.Add(Transition);
        StateHandlers.Add(Transition);

        ChangeState(GameState.PreGame);
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

        if (nextState == GameState.PreGame)
        {
            if (OnGameEnd != null)
                OnGameEnd(this, null);
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

    #region Interfaces

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

    #endregion
}
