using UnityEngine;
using System.Collections;

public class UiGameState : MonoBehaviour
{
    protected Game Controller { get; private set; }

    public void Begin(Game controller)
    {
        gameObject.SetActive(true);
        Controller = controller;
        OnBegin();
    }

    public void End()
    {
        gameObject.SetActive(false);
        OnEnd();
    }

    protected virtual void OnBegin()
    {

    }

    protected virtual void OnEnd()
    {

    }
}
