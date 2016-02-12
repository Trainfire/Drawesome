using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System;
using System.Linq;

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
        Views = new Dictionary<GameState, UiGameState>();

        Views.Add(GameState.RoundBegin, StateViews.RoundBegin);
        Views.Add(GameState.Drawing, StateViews.Drawing);
        Views.Add(GameState.Answering, StateViews.Answering);
        Views.Add(GameState.Choosing, StateViews.Choosing);
        Views.Add(GameState.Results, StateViews.Results);

        Client.Instance.MessageHandler.OnStateChange += ChangeState;
        Client.Instance.MessageHandler.OnSetTimer += OnSetTimer;
        Client.Instance.MessageHandler.OnReceivePrompt += OnRecievePrompt;
        Client.Instance.MessageHandler.OnRecieveChoices += OnRecieveChoices;
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
                    Client.Instance.Messenger.SubmitAnswer(answer);
                });

                break;

            case GameState.Choosing:

                StateViews.Choosing.OnChoiceSelected += ((choice) =>
                {
                    Client.Instance.Messenger.SubmitChoice(choice.Text.text);
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

    #endregion

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
