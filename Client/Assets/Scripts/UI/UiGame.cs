using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Protocol;
using System;

public class UiGame : UiBase, Game.IGameStateHandler, Game.IGameTransitionhandler
{
    public float FadeOutDuration = 1f;
    public float FadeInDuration = 1f;

    bool IsOut { get; set; }

    CanvasGroup canvas;
    CanvasGroup Canvas
    {
        get
        {
            if (canvas == null)
                canvas = gameObject.GetOrAddComponent<CanvasGroup>();
            return canvas;
        }
    }

    void Game.IGameStateHandler.HandleState(GameState state)
    {
        if(IsOut)
        {
            FadeIn();
            IsOut = false;
        }
    }

    void Game.IGameTransitionhandler.HandleTransition(ServerMessage.Game.SendTransitionPeriod transition)
    {
        IsOut = true;
        FadeOut();
    }

    void FadeOut()
    {
        StopAllCoroutines();
        Canvas.Fade(1f, 0f, FadeOutDuration);
    }

    void FadeIn()
    {
        StopAllCoroutines();
        Canvas.Fade(0f, 1f, FadeInDuration);
    }
}
