using UnityEngine;
using System.Collections;

public class UiGameState : MonoBehaviour
{
    public void Begin()
    {
        gameObject.SetActive(true);
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

    public virtual void RemoveAllListeners()
    {

    }
}
