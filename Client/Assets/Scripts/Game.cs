using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System;

[Serializable]
public class GameStateViews
{
    public UiGameState RoundBegin;
    public UiGameStateDrawing Drawing;
    public UiGameStateAnswering Answering;
    public UiGameStateChoosing Choosing;
    public UiGameStateResults Results;
}

public class Game : MonoBehaviour
{
    public Timer Timer;

    public GameStateViews StateViews; 

    // Data
    public string Prompt { get; private set; }
    public List<string> Choices { get; private set; }

    Dictionary<GameState, UiGameState> Views { get; set; }
    UiGameState CurrentView { get; set; }

    void Start()
    {
        Prompt = "";
        Choices = new List<string>();

        Views = new Dictionary<GameState, UiGameState>();

        Views.Add(GameState.RoundBegin, StateViews.RoundBegin);
        Views.Add(GameState.Drawing, StateViews.Drawing);
        Views.Add(GameState.Answering, StateViews.Answering);
        Views.Add(GameState.Choosing, StateViews.Choosing);
        Views.Add(GameState.Results, StateViews.Results);

        Client.Instance.MessageHandler.OnStateChange += MessageHandler_OnStateChange;
        Client.Instance.MessageHandler.OnRecievePrompt += MessageHandler_OnRecievePrompt;
        Client.Instance.MessageHandler.OnRecieveChoices += MessageHandler_OnRecieveChoices;
        Client.Instance.MessageHandler.OnRecieveResult += MessageHandler_OnRecieveResult;
        // Client.Instance.MessageHandler.OnAddTimer += MessageHandler_OnAddTimer;
    }

    void MessageHandler_OnRecieveResult(ServerMessage.Game.SendResult message)
    {
        ChangeState(GameState.Results);
    }

    void MessageHandler_OnRecieveChoices(ServerMessage.Game.SendChoices message)
    {
        Choices = message.Choices;
    }

    #region Handlers

    void MessageHandler_OnRecievePrompt(ServerMessage.Game.SendPrompt message)
    {
        Prompt = message.Prompt;
    }

    void MessageHandler_OnStateChange(ServerMessage.Game.StateChange message)
    {
        ChangeState(message.GameState);
    }

    #endregion

    void AddTimer()
    {
        Timer.Show();
    }

    void ChangeState(GameState state)
    {
        Timer.Hide();

        // Change UI state here
        if (CurrentView != null)
            CurrentView.End();

        CurrentView = Views[state];
        CurrentView.Begin(this);
    }

    public void SubmitDrawing(Texture2D texture)
    {
        // TODO: Encode to bytes then send image here
        Client.Instance.Messenger.SendImage(new byte[0]);
    }

    public void SubmitAnswer(string answer)
    {
        Client.Instance.Messenger.SubmitAnswer(answer);
    }
}
