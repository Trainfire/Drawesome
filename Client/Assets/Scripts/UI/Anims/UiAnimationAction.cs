using UnityEngine;
using System.Collections;
using System;

public class UiAnimationAction : UiAnimationComponent
{
    Action Action { get; set; }
    string Name { get; set; }

    public UiAnimationAction(string name, Action action)
    {
        Name = string.IsNullOrEmpty(name) ? "Unnamed Action" : name;
        Action = action;
    }

    public override void Play()
    {
        if (Action == null)
        {
            Debug.LogWarningFormat("Playing UiAnimationAction with no assigned action");
        }
        else
        {
            Debug.LogFormat("Play action: {0}", Name);
            Action();
        }

        Done();
    }
}
