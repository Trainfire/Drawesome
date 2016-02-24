using UnityEngine;
using System;
using System.Collections.Generic;
using Protocol;

public class UserInterface : MonoBehaviour, IClientHandler
{
    public UiLogin ViewLogin;
    public UiBrowser ViewBrowser;
    public UiRoom ViewRoom;
    public UiChat ViewChat;

    Client Client { get; set; }

    List<UiBase> views = new List<UiBase>();

    public void Initialise(Client client)
    {
        Client = client;

        views.Add(ViewLogin);
        views.Add(ViewBrowser);
        views.Add(ViewRoom);

        // Inject dependencies
        views.ForEach(x => x.Initialise(client));
        ViewChat.Initialise(client);

        ChangeMenu(ViewLogin);

        client.MessageHandler.OnServerConnectionSuccess += OnServerCompleteConnectionRequest;
        client.MessageHandler.OnServerNotifyRoomJoin += OnServerNotifyRoomJoin;
        client.MessageHandler.OnServerNotifyRoomLeave += OnServerNotifyRoomLeave;
    }

    void OnServerNotifyRoomLeave(ServerMessage.NotifyRoomLeave message)
    {
        // TODO: Display popup notice
        //if (message.Reason == RoomLeaveReason.Kicked)
        //...

        ChangeMenu(ViewBrowser);
    }

    void OnServerCompleteConnectionRequest(ServerMessage.NotifyConnectionSuccess message)
    {
        ChangeMenu(ViewBrowser);
    }

    void OnServerNotifyRoomJoin(ServerMessage.NotifyRoomJoin message)
    {
        if (message.Notice == RoomNotice.None)
            ChangeMenu(ViewRoom);
    }

    void OnDisconnect(object sender, EventArgs e)
    {
        ChangeMenu(ViewLogin);
    }

    void ChangeMenu(UiBase viewNext)
    {
        foreach (var view in views)
        {
            if (view != viewNext)
                view.Hide();
        }

        viewNext.Show();
    }

    void Log(string message, params object[] args)
    {
        var str = string.Format(message, args);
        Debug.Log("UI: " + str);
    }
}
