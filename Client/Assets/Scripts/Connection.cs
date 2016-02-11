using UnityEngine;
using System;
using WebSocketSharp;
using Protocol;
using System.Collections.Generic;

public class Connection : MonoBehaviour
{
    public delegate void MessageDelegate(object sender, MessageEventArgs message);
    public event MessageDelegate MessageRecieved;

    public event EventHandler<CloseEventArgs> ConnectionClosed;

    public PlayerData Data { get; private set; }

    WebSocket Socket { get; set; }
    string PlayerName { get; set; }
    Queue<MessageEventArgs> MessageQueue { get; set; }

    void Awake()
    {
        Data = new PlayerData();
        Socket = new WebSocket(Settings.HostUrl);
        MessageQueue = new Queue<MessageEventArgs>();
    }

    void Update()
    {
        if (MessageQueue.Count != 0 && MessageRecieved != null)
            MessageRecieved(this, MessageQueue.Dequeue());
    }

    public void Connect(string playerName)
    {
        PlayerName = playerName;

        Socket.OnMessage += OnMessage;
        Socket.OnClose += OnClose;
        Socket.Connect();
    }

    public void Disconnect()
    {
        Socket.Close();
    }

    public void Disconnect(CloseStatusCode closeStatus, string reason = "")
    {
        Socket.Close(closeStatus, reason);
    }

    void OnMessage(object sender, MessageEventArgs e)
    {
        var obj = JsonHelper.FromJson<Message>(e.Data);

        Message.IsType<ServerMessage.ConnectionSuccess>(e.Data, (data) =>
        {
            Data.ID = data.ID;
            Data.Name = PlayerName;

            Debug.Log("Recieved ID: " + Data.ID);

            SendMessage(new ClientMessage.RequestConnection(Data.ID, Data.Name));
        });

        MessageQueue.Enqueue(e);
    }

    void OnClose(object sender, CloseEventArgs e)
    {
        Socket.OnClose -= OnClose;
        Socket.OnMessage -= OnMessage;

        if (ConnectionClosed != null)
            ConnectionClosed(this, e);
    }

    public void SendMessage(Message message)
    {
        var json = JsonHelper.ToJson(message);
        Socket.Send(json);
    }
}
