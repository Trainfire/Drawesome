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
        if (currentState == GameState.Drawing)
        {
            var title = reason == GameStateEndReason.TimerExpired ? Strings.TimesUp : Strings.AllPlayersDone;
            View.Show(title, Strings.StateEndDrawing);
        }
    }

    void Game.IGameStateHandler.HandleState(GameState state)
    {
        View.Hide();
    }
}
