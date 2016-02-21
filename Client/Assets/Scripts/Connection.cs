using UnityEngine;
using System;
using Protocol;
using System.Collections.Generic;

public class Connection : MonoBehaviour
{
    public delegate void MessageDelegate(string json);
    public event MessageDelegate MessageRecieved;

    public event EventHandler ConnectionClosed;

    public PlayerData Player { get; private set; }
    public RoomData Room { get; private set; }

    public bool isRoomOwner;

    WebSocket Socket { get; set; }
    string PlayerName { get; set; }
    Queue<string> MessageQueue { get; set; }

    void Awake()
    {
        Player = new PlayerData();
        Room = new RoomData();
        MessageQueue = new Queue<string>();
    }

    void Update()
    {
        if (Socket != null)
        {
            var message = Socket.RecvString();

            if (message != null)
                OnMessage(message);

            if (Socket.error != null)
            {
                OnError(Socket.error);
            }
        }

        if (MessageQueue.Count != 0 && MessageRecieved != null)
            MessageRecieved(MessageQueue.Dequeue());

        isRoomOwner = IsRoomOwner();
    }

    public void Connect(string playerName)
    {
        //Connect(playerName, SettingsLoader.Settings.HostUrl);
        Connect(playerName, "ws://127.0.0.1:8181");
    }

    public void Connect(string playerName, string url)
    {
        PlayerName = playerName;

        Debug.LogFormat("Connect to: {0}", url);

        // Make sure existing connection is closed and events are unhooked.
        Disconnect();

        Socket = new WebSocket(new Uri(url));
        //Socket.RecvString();
        //Socket.OnClose += OnClose;
        //Socket.OnError += OnError;
        StartCoroutine(Socket.Connect());
    }

    public void Disconnect()
    {
        Disconnect("");
    }

    public void Disconnect(string reason)
    {
        if (Socket != null)
            Socket.Close();
    }

    void OnMessage(string json)
    {
        Message.IsType<ServerMessage.RequestClientName>(json, (data) =>
        {
            //var message = new ClientMessage.GiveName("SomeName");
            //var s = JsonUtility.ToJson(message);
            //Debug.Log(s);
            //Socket.SendString(s);

            SendMessage(new ClientMessage.GiveName(PlayerName));
        });

        Message.IsType<ServerMessage.UpdatePlayerInfo>(json, (data) =>
        {
            Player = data.PlayerData;
        });

        Message.IsType<ServerMessage.AssignRoomId>(json, (data) =>
        {
            Player.RoomId = data.RoomId;

            Debug.LogFormat("Recieved Room ID: {0}", data.RoomId);
        });

        Message.IsType<ServerMessage.RoomUpdate>(json, (data) =>
        {
            Room = data.RoomData;
        });

        MessageQueue.Enqueue(json);
    }

    void OnError(string error)
    {
        //Debug.LogErrorFormat("Error: {0}", error);
    }

    void OnClose(object sender)
    {
        if (ConnectionClosed != null)
            ConnectionClosed(this, null);
    }

    void OnApplicationQuit()
    {
        Disconnect("Application Quit");
    }

    public void SendMessage(Message message)
    {
        var json = JsonUtility.ToJson(message);
        Socket.Send(System.Text.Encoding.UTF8.GetBytes(json));
    }

    //public void SendMessage(Message message)
    //{
    //    var json = JsonHelper.ToJson(message);
    //    Socket.SendString(json);
    //}

    public bool IsRoomOwner()
    {
        return Room.Owner.ID == Player.ID;
    }
}
