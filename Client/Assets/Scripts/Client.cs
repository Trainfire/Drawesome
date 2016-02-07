using UnityEngine;
using System;
using System.Collections;
using WebSocketSharp;
using Protocol;
using System.Collections.Generic;

public class Client : Singleton<Client>
{
    public event EventHandler OnConnect;
    public event EventHandler OnDisconnect;
    public event EventHandler OnLeave;

    public PlayerData PlayerData { get; private set; }
    public RoomData RoomData { get; private set; }
    public string PlayerName { get; private set; }
    public List<RoomData> Rooms { get; private set; }
    public List<PlayerData> Players;
    public MessageHandler MessageHandler { get; private set; }

    const string URI = "ws://127.0.0.1:8181/room";

    WebSocket webSocket;
    Queue<string> messageQueue = new Queue<string>();

    protected override void Awake()
    {
        base.Awake();
        webSocket = new WebSocket(URI);
        Rooms = new List<RoomData>();

        MessageHandler = new MessageHandler();
        MessageHandler.OnServerNotifyPlayerAction += OnServerNotifyPlayerAction;
        MessageHandler.OnServerCompleteConnectionRequest += OnServerCompleteConnectionRequest;
        MessageHandler.OnServerUpdate += OnServerUpdate;
        MessageHandler.OnRecieveRoomList += OnRecieveRoomList;
        MessageHandler.OnChat += OnChat;
        MessageHandler.OnServerNotifyRoomError += OnRoomNotification;
        MessageHandler.OnRoomUpdate += OnRoomUpdate;
    }

    void OnRoomUpdate(ServerMessage.RoomUpdate message)
    {
        Debug.LogFormat("Recieved data for room {0}", message.RoomData.ID);
        Debug.LogFormat("Current Players: {0}", GetPlayerList(message.RoomData.Players));
        RoomData = message.RoomData;
    }

    public void Connect(string playerName)
    {
        // TODO: Send PlayerJoin message here, passing in the player's name.
        PlayerName = playerName;

        webSocket.OnMessage += OnMessage;
        webSocket.OnOpen += OnOpen;
        webSocket.OnClose += OnClose;
        webSocket.Connect();

        if (OnConnect != null)
            OnConnect(this, null);
    }

    public void Disconnect()
    {
        webSocket.Close(CloseStatusCode.Normal);
        webSocket.OnClose -= OnClose;
        webSocket.OnOpen -= OnOpen;
        webSocket.OnMessage -= OnMessage;

        if (OnDisconnect != null)
            OnDisconnect(this, null);
    }

    public void CreateRoom(string password = "")
    {
        SendMessage(new ClientMessage.CreateRoom(PlayerData, password));
    }

    public void JoinRoom(string roomId, string password = "")
    {
        Debug.LogFormat("Attempt to join room {0}", roomId);
        SendMessage(new ClientMessage.JoinRoom(PlayerData, roomId, password));
    }

    public void LeaveRoom()
    {
        SendMessage(new ClientMessage.LeaveRoom(PlayerData));
    }

    public void RequestRooms()
    {
        SendMessage(new ClientMessage.RequestRoomList(PlayerData));
    }

    public void Say(string message)
    {
        SendMessage(new SharedMessage.Chat(PlayerData, message));
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
        //Debug.LogFormat("{0} : Send message: {1}", DateTime.Now, json);
        webSocket.Send(json);
    }

    void OnClose(object sender, CloseEventArgs e)
    {
        Debug.LogFormat("Connection closed ({0})", e.Reason);
        webSocket.OnOpen -= OnOpen;
    }

    void OnOpen(object sender, EventArgs e)
    {

    }

    void OnMessage(object sender, MessageEventArgs e)
    {
        //Debug.LogFormat("Recieved message: {0}", e.Data);
        messageQueue.Enqueue(e.Data);
    }

    void Update()
    {
        if (messageQueue.Count != 0)
            MessageHandler.HandleMessage(messageQueue.Dequeue());
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }

    public bool IsConnected()
    {
        return webSocket.ReadyState == WebSocketState.Open ? true : false;
    }

    #region Message Handlers

    void OnServerNotifyPlayerAction(ServerMessage.NotifyPlayerAction message)
    {
        var str = StringFormatter.FormatPlayerAction(message, PlayerData);
        Debug.Log(str);

        // Fire event if client has left.
        if (message.Player.ID == PlayerData.ID)
            OnLeave(this, null);
    }

    void OnRecieveRoomList(ServerMessage.RoomList message)
    {
        Rooms = new List<RoomData>();

        foreach (var room in message.Rooms)
        {
            Debug.LogFormat("Recieved room with ID {0}", room.ID);
            Rooms.Add(room);
        }
    }

    void OnServerCompleteConnectionRequest(ServerMessage.ConnectionSuccess message)
    {
        PlayerData = new PlayerData();
        PlayerData.ID = message.ID;
        PlayerData.Name = PlayerName;
        Debug.Log("Recieved ID: " + PlayerData.ID);
        var playerConnectMessage = new ClientMessage.RequestConnection(PlayerData.ID.ToString(), PlayerName);
        var json = JsonUtility.ToJson(playerConnectMessage);
        webSocket.Send(json);
    }

    void OnServerUpdate(ServerUpdate update)
    {
        Debug.LogFormat("Recieved Server Update with {0} players.", update.Players.Count);
        Players = update.Players;
        Debug.LogFormat("Connected players: {0}", GetPlayerList(Players));
    }

    void OnChat(SharedMessage.Chat chat)
    {
        Debug.LogFormat("{0}: {1}", chat.Player.Name, chat.Message);
    }

    void OnRoomNotification(ServerMessage.NotifyRoomError notification)
    {
        Debug.LogFormat("Room Notification: {0}", notification.Notice);
    }

    string GetPlayerList(List<PlayerData> players)
    {
        string names = "";

        for (int i = 0; i < players.Count; i++)
        {
            names += i != players.Count - 1 ? players[i].Name + ", " : players[i].Name;
        }

        return names;
    }

    #endregion
}
