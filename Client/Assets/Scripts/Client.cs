using UnityEngine;
using System;
using System.Collections.Generic;
using Protocol;

public class Client : MonoBehaviour
{
    public bool LogMessages;

    public Connection Connection { get; private set; }
    public MessageHandler MessageHandler { get; private set; }
    public Messenger Messenger { get; private set; }

    public void Initialise()
    {
        Connection = gameObject.AddComponent<Connection>();
        MessageHandler = new MessageHandler(Connection);
        Messenger = new Messenger(Connection);

        MessageHandler.OnAny += MessageHandler_OnMessage;
    }

    void MessageHandler_OnMessage(Message message)
    {
        if (LogMessages)
            Debug.LogFormat("{0} - Recieved Message - Type: {1}, Json: {2}", DateTime.Now.ToShortTimeString(), message.Identity, message.AsJson());
    }
}
