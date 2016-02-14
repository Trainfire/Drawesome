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

    public PlayerData Player { get; private set; }
    public RoomData Room { get; private set; }

    public bool isRoomOwner;

    WebSocket Socket { get; set; }
    string PlayerName { get; set; }
    Queue<MessageEventArgs> MessageQueue { get; set; }

    void Awake()
    {
        Player = new PlayerData();
        Room = new RoomData();
        Socket = new WebSocket(Settings.HostUrl);
        MessageQueue = new Queue<MessageEventArgs>();
    }

    void Update()
    {
        if (MessageQueue.Count != 0 && MessageRecieved != null)
            MessageRecieved(this, MessageQueue.Dequeue());

        isRoomOwner = IsRoomOwner();
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
        Message.IsType<ServerMessage.ConnectionSuccess>(e.Data, (data) =>
        {
            Player.ID = data.ID;
            Player.Name = PlayerName;

            Debug.Log("Recieved ID: " + Player.ID);

            SendMessage(new ClientMessage.RequestConnection(Player.ID, Player.Name));
        });

        Message.IsType<ServerMessage.AssignRoomId>(e.Data, (data) =>
        {
            Player.RoomId = data.RoomId;

            Debug.LogFormat("Recieved Room ID: {0}", data.RoomId);
        });

        Message.IsType<ServerMessage.RoomUpdate>(e.Data, (data) =>
        {
            Room = data.RoomData;
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

    void OnApplicationQuit()
    {
        Disconnect(CloseStatusCode.Normal, "Application Quit");
    }

    public void SendMessage(Message message)
    {
        var json = JsonHelper.ToJson(message);
        Socket.Send(json);
    }

    public bool IsRoomOwner()
    {
        return Room.Owner.ID == Player.ID;
    }
}
