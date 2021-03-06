using UnityEngine;
using System.Collections;

public abstract class State
{
    protected UiBase View { get; private set; }
    protected Client Client { get; private set; }

    bool IsFirstShow { get; set; }
    bool IsActive { get; set; }

    public State(Client client, UiBase view)
    {
        Client = client;
        Client.MessageHandler.OnMessage += HandleMessage;
        View = view;
        View.Initialise(client);
        IsFirstShow = true;
    }

    public void Begin()
    {
        View.gameObject.SetActive(true);
        View.Show();

        if (IsFirstShow)
        {
            OnFirstBegin();
            IsFirstShow = false;
        }

        IsActive = true;
        OnBegin();
    }

    public void End()
    {
        View.gameObject.SetActive(false);
        View.Hide();
        OnEnd();
        IsActive = false;
    }

    protected T GetView<T>() where T : UiBase
    {
        return View as T;
    }

    protected void GetView<T>(System.Action<T> OnGet) where T : UiBase
    {
        OnGet(View as T);
    }

    protected virtual void OnFirstBegin()
    {

    }

    protected virtual void OnBegin()
    {

    }

    void HandleMessage(string json)
    {
        if (IsActive)
            OnMessage(json);
    }

    protected virtual void OnMessage(string json)
    {

    }

    protected virtual void OnEnd()
    {

    }

    public virtual void Update()
    {

    }
}
