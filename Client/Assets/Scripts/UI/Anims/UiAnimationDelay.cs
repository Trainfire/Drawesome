using UnityEngine;
using System.Collections;
using System;

public class UiAnimationDelay : UiAnimationComponent
{
    float Delay { get; set; }
    UiAnimationController Controller { get; set; }

    public UiAnimationDelay(UiAnimationController controller, float delay)
    {
        Controller = controller;
        Delay = delay;
    }

    public override void Play()
    {
        Playing = true;
        Controller.StartCoroutine(OnPlay());
    }

    IEnumerator OnPlay()
    {
        yield return new WaitForSeconds(Delay);
        Done();
    }
}
