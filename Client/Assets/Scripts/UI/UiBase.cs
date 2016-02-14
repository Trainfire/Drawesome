using UnityEngine;
using System.Collections;
using System;

public class UiBase : MonoBehaviour, IClientHandler
{
    protected Client Client { get; private set; }

    bool IsFirstShow { get; set; }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        OnHide();
    }

    public virtual void Initialise(Client client)
    {
        Client = client;
        IsFirstShow = true;
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);

        if (IsFirstShow)
        {
            OnFirstShow();
            IsFirstShow = false;
        }

        OnShow();
    }

    protected virtual void OnFirstShow()
    {

    }

    protected virtual void OnShow()
    {

    }

    protected virtual void OnHide()
    {
        
    }
}
