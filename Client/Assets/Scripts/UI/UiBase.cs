using UnityEngine;
using System.Collections;
using System;

public class UiBase : MonoBehaviour, IClientHandler
{
    protected Client Client { get; private set; }

    public virtual void Hide()
    {
        OnHide();
    }

    public virtual void Initialise(Client client)
    {
        Client = client;
    }

    public virtual void Show()
    {
        OnShow();
    }

    protected virtual void OnHide()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnShow()
    {
        gameObject.SetActive(true);
    }
}
