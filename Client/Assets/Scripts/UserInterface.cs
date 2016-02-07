using UnityEngine;
using System;
using System.Collections.Generic;
using Protocol;

public class UserInterface : MonoBehaviour
{
    public UiLogin ViewLogin;
    public UiBrowser ViewBrowser;
    public UiRoom ViewRoom;

    List<UiMenu> views = new List<UiMenu>();

    UiMenu viewCurrent;

    bool isChangingMenu = false;

    void Start()
    {
        views.Add(ViewLogin);
        views.Add(ViewBrowser);
        views.Add(ViewRoom);

        ChangeMenu(ViewLogin);

        Client.Instance.MessageHandler.OnServerCompleteConnectionRequest += OnServerCompleteConnectionRequest;
        Client.Instance.OnDisconnect += OnDisconnect;
        Client.Instance.MessageHandler.OnRoomUpdate += OnRoomUpdate;
        Client.Instance.OnLeave += OnLeave;
    }

    void OnLeave(object sender, EventArgs e)
    {
        ChangeMenu(ViewBrowser);
    }

    void OnServerCompleteConnectionRequest(Protocol.ServerMessage.ConnectionSuccess message)
    {
        ChangeMenu(ViewBrowser);
    }

    void OnRoomUpdate(Protocol.ServerMessage.RoomUpdate message)
    {
        ChangeMenu(ViewRoom);
    }

    void OnDisconnect(object sender, EventArgs e)
    {
        ChangeMenu(ViewLogin);
    }

    void ChangeMenu(UiMenu viewNext)
    {
        foreach (var view in views)
        {
            if (view != viewNext)
                view.Hide();
        }

        viewNext.Show();
        viewCurrent = viewNext;
    }

    void Log(string message, params object[] args)
    {
        var str = string.Format(message, args);
        Debug.Log("UI: " + str);
    }
}
