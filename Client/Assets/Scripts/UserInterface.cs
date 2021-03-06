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

    [SerializeField]
    PopupFactory popupFactory;
    public PopupFactory Popups { get { return popupFactory; } }

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
        client.Connection.ConnectionClosed += OnConnectionClosed;
        client.MessageHandler.OnServerNotifyRoomJoin += OnServerNotifyRoomJoin;
        client.MessageHandler.OnServerNotifyRoomLeave += OnServerNotifyRoomLeave;
    }

    void OnServerNotifyRoomLeave(ServerMessage.NotifyRoomLeave message)
    {
        // TODO: Display popup notice
        if (message.Reason == RoomLeaveReason.KickedForAfk)
            Popups.MakeMessagePopup(Strings.Popups.KickedForAfk).Show();

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

    void OnConnectionClosed(object sender, EventArgs e)
    {
        Popups.MakeMessagePopup(Strings.Popups.ConnectionError, () => ChangeMenu(ViewLogin)).Show();
    }

    void ChangeMenu(UiBase viewNext)
    {
        var animController = gameObject.GetOrAddComponent<UiAnimationController>();

        foreach (var view in views)
        {
            if (view != viewNext)
            {
                var temp = view;
                animController.AddAnim(new UiAnimationFade(view.gameObject, 0.2f, UiAnimationFade.FadeType.Out), false);
                animController.AddAction("", () => temp.Hide());
            }
        }

        animController.AddAnim(new UiAnimationFade(viewNext.gameObject, 0.2f, UiAnimationFade.FadeType.In), false);
        animController.AddAction("", () => viewNext.Show());

        animController.PlayAnimations();
    }

    void Log(string message, params object[] args)
    {
        var str = string.Format(message, args);
        Debug.Log("UI: " + str);
    }
}
