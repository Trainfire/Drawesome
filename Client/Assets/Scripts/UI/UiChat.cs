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

    bool shouldPostChat = false;
    SharedMessage.Chat cachedChat;

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
        cachedChat = message;
        shouldPostChat = true;
    }

    void OnSend()
    {
        Client.Instance.Say(Input.text);
        Input.text = "";
    }

    void Update()
    {
        // TODO: Rewrite
        if (shouldPostChat)
        {
            var instance = UiUtility.AddChild<Text>(MessagesContainer, MessagePrototype);
            instance.text = string.Format("{0}: {1}", cachedChat.Player.Name, cachedChat.Message);
            instance.enabled = true;
            shouldPostChat = false;
        }

        ScrollRect.normalizedPosition = new Vector2(ScrollRect.normalizedPosition.x, 0);
    }
}
