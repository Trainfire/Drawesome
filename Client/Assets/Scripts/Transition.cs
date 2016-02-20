using UnityEngine;
using System.Collections;
using Protocol;
using System;

public class Transition : MonoBehaviour, Game.IGameStateEndHandler, Game.IGameStateHandler
{
    public UiTransition View;

    string Title { get; set; }
    string Message { get; set; }
    bool Show { get; set; }

    void Awake()
    {
        View.Hide();
    }

    void Game.IGameStateEndHandler.HandleStateEnd(GameState currentState, GameStateEndReason reason)
    {
        if (reason == GameStateEndReason.TimerExpired)
        {
            View.Show(Strings.TimesUp);
        }
    }

    void Game.IGameStateHandler.HandleState(GameState state)
    {
        View.Hide();
    }
}
