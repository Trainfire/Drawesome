using UnityEngine;
using UnityEngine.UI;
using Protocol;

public class UiChat : UiBase
{
    public Text MessagePrototype;
    public RectTransform MessagesContainer;
    public InputField Input;
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

        Send.onClick.AddListener(() => OnSend());
    }

    void OnServerNotifyPlayerAction(ServerMessage.NotifyPlayerAction message)
    {
        AddMessage(StringFormatter.FormatPlayerAction(message, Client.Connection.Data));
    }

    void OnChat(SharedMessage.Chat message)
    {
        AddMessage("{0}: {1}", message.Player.Name, message.Message);
    }

    void AddMessage(string message, params object[] args)
    {
        var instance = UiUtility.AddChild<Text>(MessagesContainer, MessagePrototype);
        instance.text = string.Format(message, args);
        instance.enabled = true;

        ScrollRect.normalizedPosition = new Vector2(ScrollRect.normalizedPosition.x, 0);
    }

    void OnSend()
    {
        Client.Messenger.Say(Input.text);
        Input.text = "";
    }
}
