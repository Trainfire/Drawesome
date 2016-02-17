using UnityEngine;
using System;

public class UiTweenFloat : UiTween<float>
{
    public float Value;
    public float From;
    public float To;

    protected override float OnTween(float t)
    {
        return Mathf.Lerp(From, To, t);
    }
}
