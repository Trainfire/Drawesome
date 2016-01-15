using UnityEngine;
using System;
using WebSocketSharp;
using Protocol;

public class Client : Singleton<Client>
{
    const string URI = "ws://127.0.0.1:8181/room";

    WebSocket webSocket;

    protected override void Awake()
    {
        base.Awake();
        webSocket = new WebSocket(URI);
    }

    public void Connect()
    {
        webSocket.Connect();
        webSocket.OnMessage += OnMessage;
    }

    public void Disconnect()
    {
        webSocket.Close(CloseStatusCode.Normal);
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
        Debug.LogFormat("{0} : Send message...", DateTime.Now);
        webSocket.Send(json);
    }

    void OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log("Recieved message: " + e.Data);
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
