using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System;
using System.Linq;

[Serializable]
public class GameStateViews
{
    public UiGameStateRoundBegin RoundBegin;
    public UiGameStateDrawing Drawing;
    public UiGameStateAnswering Answering;
    public UiGameStateChoosing Choosing;
    public UiGameStateResults Results;
    public UiGameStateRoundEnd RoundEnd;
}

public class Game : MonoBehaviour, IClientHandler
{
    public Timer Timer;
    public GameStateViews StateViews; 

    Client Client { get; set; }
    Dictionary<GameState, UiGameState> Views { get; set; }
    UiGameState CurrentView { get; set; }

    public void Initialise(Client client)
    {
        Client = client;

        Views = new Dictionary<GameState, UiGameState>();

        Views.Add(GameState.RoundBegin, StateViews.RoundBegin);
        Views.Add(GameState.Drawing, StateViews.Drawing);
        Views.Add(GameState.Answering, StateViews.Answering);
        Views.Add(GameState.Choosing, StateViews.Choosing);
        Views.Add(GameState.Results, StateViews.Results);
        Views.Add(GameState.RoundEnd, StateViews.RoundEnd);

        foreach (var view in Views.Values)
        {
            view.Hide();
        }

        Client.MessageHandler.OnStateChange += ChangeState;
        Client.MessageHandler.OnSetTimer += OnSetTimer;
        Client.MessageHandler.OnReceivePrompt += OnRecievePrompt;
        Client.MessageHandler.OnRecieveChoices += OnRecieveChoices;
        Client.MessageHandler.OnRecieveResult += OnRecieveResult;
    }

    #region Handlers

    /// <summary>
    /// Change the UI currently being shown.
    /// </summary>
    /// <param name="message"></param>
    void ChangeState(ServerMessage.Game.StateChange message)
    {
        Timer.Hide();

        if (CurrentView != null)
        {
            CurrentView.RemoveAllListeners();
            CurrentView.End();
        }

        OnState(message.GameState);

        CurrentView = Views[message.GameState];
        CurrentView.Begin();
    }

    /// <summary>
    /// Add logic for specific states here.
    /// </summary>
    /// <param name="state"></param>
    void OnState(GameState state)
    {
        switch (state)
        {
            case GameState.Answering:

                StateViews.Answering.Submit.onClick.AddListener(() =>
                {
                    var answer = StateViews.Answering.InputField.text;
                    Client.Messenger.SubmitAnswer(answer);
                });

                break;

            case GameState.Choosing:

                StateViews.Choosing.OnChoiceSelected += ((choice) =>
                {
                    Client.Messenger.SubmitChoice(choice.Text.text);
                });

                break;
        };
    }

    void OnSetTimer(ServerMessage.Game.SetTimer message)
    {
        Timer.Show();
        Timer.SetTime(message.Time);
    }

    void OnRecievePrompt(ServerMessage.Game.SendPrompt message)
    {
        StateViews.Drawing.SetPrompt(message.Prompt);
    }

    void OnRecieveChoices(ServerMessage.Game.SendChoices message)
    {
        StateViews.Choosing.ShowChoices(message.Choices);
    }

    void OnRecieveResult(ServerMessage.Game.SendResult message)
    {
        StateViews.Results.ShowResult(message.Result);
    }

    #endregion
}
