using UnityEngine;
using System;
using System.Collections.Generic;
using Protocol;

public class Client : MonoBehaviour
{
    public bool enableLogging = true;
    public bool EnableLogging
    {
        set
        {
            enableLogging = value;
            Debug.LogFormat("Logging enabled: " + value);
        }
    }

    public PlayerData PlayerData { get { return Connection.Player; } }
    public Connection Connection { get; private set; }
    public MessageHandler MessageHandler { get; private set; }
    public Messenger Messenger { get; private set; }
    public Settings Settings { get; private set; }

    public void Initialise()
    {
        Connection = gameObject.AddComponent<Connection>();
        MessageHandler = new MessageHandler(Connection);
        Messenger = new Messenger(Connection);

        MessageHandler.OnAny += MessageHandler_OnMessage;
    }

    void MessageHandler_OnMessage(Message message)
    {
        if (enableLogging)
            Debug.LogFormat("{0} - Recieved Message - Type: {1}, Json: {2}", DateTime.Now.ToShortTimeString(), message.Identity, message.AsJson());
    }

    #region Helpers

    public bool IsPlayer(PlayerData otherPlayerData)
    {
        return PlayerData.ID == otherPlayerData.ID;
    }

    public static bool IsWebGL()
    {
        #if UNITY_WEBGL
            return true;
        #else
            return false;
        #endif
    }

#endregion
}
