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

        Send.onClick.AddListener(() => OnSend());
    }

    void OnServerNotifyPlayerAction(ServerMessage.NotifyPlayerAction message)
    {
        AddMessage(StringFormatter.FormatPlayerAction(message, Client.Connection.Player));
    }

    void OnChat(SharedMessage.Chat message)
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
        Client.Messenger.Say(InputField.text);
        InputField.text = "";
        InputField.Focus();
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Return))
            OnSend();
    }
}
