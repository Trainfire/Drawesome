using UnityEngine;
using System.Collections;
using System;

public class UiAnimationFade : UiAnimationComponent
{
    public enum FadeType
    {
        In,
        Out,
    }
    public FadeType Fade;

    float Duration { get; set; }
    CanvasGroup Canvas { get; set; }

    public UiAnimationFade(GameObject gameobject, float duration, FadeType fadeType) : base(gameobject)
    {
        Canvas = gameobject.GetOrAddComponent<CanvasGroup>();
        Canvas.alpha = 0f;
        Duration = duration;
        Fade = fadeType;
    }

    public override void Play()
    {
        base.Play();
        
        var tweener = Target.AddComponent<UiTweenFloat>();

        switch (Fade)
        {
            case FadeType.In:
                tweener.From = 0f;
                tweener.To = 1f;
                break;
            case FadeType.Out:
                tweener.From = 1f;
                tweener.To = 0f;
                break;
            default:
                break;
        }

        if (Mathf.Approximately(Duration, 0f))
        {
            Canvas.alpha = Fade == FadeType.In ? 1f : 0f;
            Done();
            return;
        }

        tweener.Duration = Duration;

        tweener.OnTweenValue += (value) =>
        {
            Canvas.alpha = value;
        };

        tweener.Play(() =>
        {
            Done();
        });
    }
}
