using UnityEngine;
using UnityEngine.UI;
using Protocol;

public class UiChat : MonoBehaviour
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

    void Start()
    {
        Client.Instance.MessageHandler.OnChat += OnChat;

        Send.onClick.AddListener(() => OnSend());
    }

    void OnChat(SharedMessage.Chat message)
    {
        var instance = UiUtility.AddChild<Text>(MessagesContainer, MessagePrototype);
        instance.text = string.Format("{0}: {1}", message.Player.Name, message.Message);
        instance.enabled = true;

        ScrollRect.normalizedPosition = new Vector2(ScrollRect.normalizedPosition.x, 0);
    }

    void OnSend()
    {
        Client.Instance.Say(Input.text);
        Input.text = "";
    }
}
