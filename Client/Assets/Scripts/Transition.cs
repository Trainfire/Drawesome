using UnityEngine;
using System.Collections;
using Protocol;
using System;

public class Transition : MonoBehaviour, Game.IGameStateEndHandler, Game.IGameStateHandler
{
    public UiTransition View;

    void Awake()
    {
        View.Hide();
    }

    void Game.IGameStateEndHandler.HandleStateEnd(GameState currentState, GameStateEndReason reason)
    {
        string message = "";

        switch (currentState)
        {
            case GameState.Drawing:
                message = Strings.StateEndDrawing;
                break;
            case GameState.Answering:
                message = Strings.StateEndAnswering;
                break;
            case GameState.Choosing:
                message = Strings.StateEndChoosing;
                break;
        }

        string title = reason == GameStateEndReason.TimerExpired ? Strings.TimesUp : Strings.AllPlayersDone;

        View.Show(title, message);
    }

    void Game.IGameStateHandler.HandleState(GameState state)
    {
        View.Hide();
    }
}
