using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Protocol;

public class UiChat : UiBase
{
    public Text MessagePrototype;
    public RectTransform MessagesContainer;
    public InputField InputField;
    public Button Send;
    public ScrollRect ScrollRect;

    void Awake()
    {
        MessagePrototype.enabled = false;
    }

    public override void Initialise(Client client)
    {
        base.Initialise(client);

        Client.MessageHandler.OnChat += OnChat;
        Client.MessageHandler.OnServerNotifyPlayerAction += OnServerNotifyPlayerAction;
        Client.MessageHandler.OnRoomCountdownCancel += OnRoomCountdownCancel;
        Client.MessageHandler.OnRoomCountdownStart += OnRoomCountdownStart;

        Send.onClick.AddListener(() => OnSend());
    }

    void OnRoomCountdownStart(ServerMessage.NotifyRoomCountdown message)
    {
        AddMessage(Strings.CountdownStart, Client.Connection.Room.Owner.Name);
    }

    void OnRoomCountdownCancel(ServerMessage.NotifyRoomCountdownCancel message)
    {
        AddMessage(Strings.CountdownCancel, Client.Connection.Room.Owner.Name);
    }

    void OnServerNotifyPlayerAction(ServerMessage.NotifyPlayerAction message)
    {
        if (message.Context == PlayerActionContext.Room)
            AddMessage(StringFormatter.FormatPlayerAction(message, Client.Connection.Player));
    }

    void OnChat(ServerMessage.NotifyChatMessage message)
    {
        AddMessage("{0}: {1}", message.Player.Name, message.Message);
    }

    void AddMessage(string message, params object[] args)
    {
        var instance = UiUtility.AddChild<Text>(MessagesContainer.gameObject, MessagePrototype);
        instance.text = string.Format(message, args);
        instance.enabled = true;

        if (gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            StartCoroutine(Hack());
        }
    }

    IEnumerator Hack()
    {
        yield return new WaitForEndOfFrame();
        ScrollRect.ScrollToBottom();
    }

    void OnSend()
    {
        if (!string.IsNullOrEmpty(InputField.text))
        {
            Client.Messenger.Say(InputField.text);
            InputField.text = "";
            InputField.Focus();
        }
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Return))
            OnSend();
    }
}
