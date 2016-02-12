using UnityEngine;
using System;
using System.Collections.Generic;
using Protocol;

public class UserInterface : MonoBehaviour, IClientHandler
{
    /// <summary>
    /// Views
    /// </summary>
    public UiLogin ViewLogin;
    public UiBrowser ViewBrowser;
    public UiRoom ViewRoom;

    public Client Client { get; set; }

    List<UiBase> views = new List<UiBase>();

    public void Initialise(Client client)
    {
        views.Add(ViewLogin);
        views.Add(ViewBrowser);
        views.Add(ViewRoom);

        // Inject dependencies
        views.ForEach(x => x.Initialise(client));

        ChangeMenu(ViewLogin);

        Client.MessageHandler.OnServerCompleteConnectionRequest += OnServerCompleteConnectionRequest;
        Client.MessageHandler.OnServerNotifyPlayerAction += OnServerNotifyPlayerAction;
        Client.MessageHandler.OnRoomUpdate += OnRoomUpdate;
    }

    void OnServerNotifyPlayerAction(ServerMessage.NotifyPlayerAction message)
    {
        // TODO: wtf?!?
        if (message.Player.ID == Client.Connection.Data.ID)
            ChangeMenu(ViewBrowser);
    }

    void OnServerCompleteConnectionRequest(ServerMessage.ConnectionSuccess message)
    {
        ChangeMenu(ViewBrowser);
    }

    void OnRoomUpdate(ServerMessage.RoomUpdate message)
    {
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
