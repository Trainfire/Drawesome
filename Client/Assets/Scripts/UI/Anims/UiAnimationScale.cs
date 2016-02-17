using UnityEngine;
using System.Collections;
using System;

public class UiAnimationScale : UiAnimationComponent
{
    float Duration { get; set; }
    Vector3 From { get; set; }
    Vector3 To { get; set; }
    CanvasGroup Canvas { get; set; }

    public UiAnimationScale(GameObject gameobject, Vector3 from, Vector3 to, float duration) : base(gameobject)
    {
        Canvas = gameobject.GetOrAddComponent<CanvasGroup>();
        Duration = duration;
        To = to;
    }

    public override void Play()
    {
        base.Play();

        var tweener = Target.AddComponent<UiTweenVector>();

        tweener.Duration = Duration;
        tweener.From = From;
        tweener.To = To;

        tweener.OnTweenValue += (value) =>
        {
            Canvas.transform.localScale = value;
        };

        tweener.Play(() =>
        {
            Done();
        });
    }
}
