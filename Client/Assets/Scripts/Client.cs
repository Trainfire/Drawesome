using UnityEngine;
using System;
using Protocol;

public class Client : Singleton<Client>
{
    public bool LogMessages;

    public Connection Connection { get; set; }
    public MessageHandler MessageHandler { get; private set; }
    public Messenger Messenger { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Connection = gameObject.AddComponent<Connection>();
        MessageHandler = new MessageHandler(Connection);
        Messenger = new Messenger(Connection);

        MessageHandler.OnAny += MessageHandler_OnMessage;

        LogMessages = true;
    }

    void MessageHandler_OnMessage(Message message)
    {
        if (LogMessages)
            Debug.LogFormat("{0} - Recieved Message - Type: {1}, Json: {2}", DateTime.Now.ToShortTimeString(), message.Type, message.AsJson());
    }
}
