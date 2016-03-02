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
            var str = Socket.RecvString();

            if (str != null)
                OnMessage(str);

            // Dumb hack for dumb plugin...
            if (Socket.error != null)
            {
                Debug.LogErrorFormat("Socket Error: {0}", Socket.error);

                Socket.Close();
                Socket = null;

                if (ConnectionClosed != null)
                    ConnectionClosed(this, null);
            }
        }

        if (MessageQueue.Count != 0 && MessageRecieved != null)
            MessageRecieved(MessageQueue.Dequeue());

        isRoomOwner = IsRoomOwner();
    }

    public void Connect(string playerName)
    {
        var url = Debug.isDebugBuild ? "ws://127.0.0.1:8181" : SettingsLoader.Settings.HostUrl;
        Connect(playerName, url);
    }

    public void Connect(string playerName, string url)
    {
        PlayerName = playerName;

        if (Socket != null)
            Disconnect();

        Socket = new WebSocket(new Uri(url));
        StartCoroutine(Socket.Connect());
    }

    public void Disconnect()
    {
        Disconnect("");
    }

    public void Disconnect(string reason)
    {
        Debug.LogFormat("Disconnected: {0}", reason);
        Socket.Close();
    }

    void OnMessage(string json)
    {
        Message.IsType<ServerMessage.RequestClientInfo>(json, (data) =>
        {
            SendMessage(new ClientMessage.GiveClientInfo(PlayerName, ProtocolInfo.Version));
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

    void OnApplicationQuit()
    {
        Disconnect("Application Quit");
    }

    public void SendMessage(Message message)
    {
        var json = JsonHelper.ToJson(message);
        Socket.SendString(json);
    }

    public bool IsRoomOwner()
    {
        return Room.Owner.ID == Player.ID;
    }
}
