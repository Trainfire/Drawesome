using UnityEngine;
using System.Collections;

public class UiGameState : UiBase
{
    public void Begin()
    {
        Show();
        OnBegin();
    }

    public void End()
    {
        Hide();
        OnEnd();
    }

    protected virtual void OnBegin()
    {

    }

    protected virtual void OnEnd()
    {

    }

    public virtual void RemoveAllListeners()
    {

    }
}
