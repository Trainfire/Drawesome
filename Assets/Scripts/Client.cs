using UnityEngine;
using System;
using WebSocketSharp;
using Protocol;

public class Client : Singleton<Client>
{
    public string ID { get; private set; }
    public string PlayerName { get; private set; }

    const string URI = "ws://127.0.0.1:8181/room";

    WebSocket webSocket;
    MessageHandler messageHandler = new MessageHandler();

    protected override void Awake()
    {
        base.Awake();
        webSocket = new WebSocket(URI);

        messageHandler.OnGeneric += (message) =>
        {
            Debug.Log("Recieved Generic Message: " + message.LogMessage);
        };

        messageHandler.OnValidatePlayer += OnValidatePlayer;
    }

    public void Connect(string playerName)
    {
        // TODO: Send PlayerJoin message here, passing in the player's name.
        PlayerName = playerName;

        webSocket.OnMessage += OnMessage;
        webSocket.OnOpen += OnOpen;
        webSocket.OnClose += OnClose;
        webSocket.Connect();
    }

    public void Disconnect()
    {
        webSocket.Close(CloseStatusCode.Normal);
        webSocket.OnClose -= OnClose;
        webSocket.OnOpen -= OnOpen;
        webSocket.OnMessage -= OnMessage;
    }

    public void SendMessage(Message message)
    {
        var json = JsonUtility.ToJson(message);
        SendJson(json);
    }

    public void SendMessage(MessageType messageType)
    {
        var message = new Message(messageType);
        var json = JsonUtility.ToJson(message);
        SendJson(json);
    }

    void SendJson(string json)
    {
        Debug.LogFormat("{0} : Send message: {1}", DateTime.Now, json);
        webSocket.Send(json);
    }

    void OnClose(object sender, CloseEventArgs e)
    {
        Debug.LogFormat("Connection closed ({0})", e.Reason);
        webSocket.OnOpen -= OnOpen;
    }

    void OnOpen(object sender, EventArgs e)
    {
        //var message = new PlayerConnectMessage(ID, PlayerName);
        //Debug.LogFormat("Connect with name: " + PlayerName);
        //var json = JsonUtility.ToJson(message);
        //Debug.Log(json);
        //webSocket.Send(json);
    }

    void OnMessage(object sender, MessageEventArgs e)
    {
        Debug.LogFormat("Recieved message: {0}", e.Data);
        messageHandler.HandleMessage(e.Data);
    }

    void OnValidatePlayer(ValidatePlayer message)
    {
        Debug.Log("OnValidatePlayer");
        ID = message.ID;
        var playerConnectMessage = new PlayerConnectMessage(ID, PlayerName);
        var json = JsonUtility.ToJson(playerConnectMessage);
        webSocket.Send(json);
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }

    public bool IsConnected()
    {
        return webSocket.ReadyState == WebSocketState.Open ? true : false;
    }
}
