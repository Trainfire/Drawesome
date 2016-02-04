using UnityEngine;
using System;
using WebSocketSharp;
using Protocol;
using System.Collections.Generic;

public class Client : Singleton<Client>
{
    public PlayerData PlayerData { get; private set; }

    public string PlayerName { get; private set; }
    public List<RoomData> Rooms { get; private set; }

    public List<PlayerData> Players;

    const string URI = "ws://127.0.0.1:8181/room";

    WebSocket webSocket;
    MessageHandler messageHandler = new MessageHandler();

    protected override void Awake()
    {
        base.Awake();
        webSocket = new WebSocket(URI);
        Rooms = new List<RoomData>();

        messageHandler.OnServerNotifyPlayerAction += OnServerNotifyPlayerAction;
        messageHandler.OnServerCompleteConnectionRequest += OnServerCompleteConnectionRequest;
        messageHandler.OnServerUpdate += OnServerUpdate;
        messageHandler.OnRecieveRoomList += OnRecieveRoomList;
        messageHandler.OnChat += OnChat;
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

    public void CreateRoom(string password = "")
    {
        SendMessage(new ClientMessage.CreateRoom(PlayerData, password));
    }

    public void JoinRoom(string roomId)
    {
        SendMessage(new ClientMessage.JoinRoom(PlayerData, roomId));
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
        messageHandler.HandleMessage(e.Data);
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
        // Determine if message is about self.
        string owner = message.Player.ID == PlayerData.ID ? "You" : message.Player.Name;
        bool isAboutSelf = message.Player.ID == PlayerData.ID;

        switch (message.Action)
        {
            case PlayerAction.None:
                break;
            case PlayerAction.Connected:
                Debug.LogFormat("{0} connected.", owner);
                break;
            case PlayerAction.Disconnected:
                Debug.LogFormat("{0} disconnected.", owner);
                break;
            case PlayerAction.Kicked:
                break;
            case PlayerAction.Joined:
                Debug.LogFormat("{0} joined the room.", owner);
                break;
            case PlayerAction.Left:
                Debug.LogFormat("{0} left.", owner);
                break;
            case PlayerAction.PromotedToOwner:
                if (isAboutSelf)
                {
                    Debug.LogFormat("{0} are now the room owner.", owner);
                }
                else
                {
                    Debug.LogFormat("{0} is now the room owner.", owner);
                }
                break;
            default:
                break;
        }
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

        string names = "";

        for (int i = 0; i < Players.Count; i++)
        {
            names += i != Players.Count - 1 ? Players[i].Name + ", " : Players[i].Name;
        }

        Debug.LogFormat("Connected players: {0}", names);
    }

    void OnChat(SharedMessage.Chat chat)
    {
        if (chat.Player.ID != PlayerData.ID)
            Debug.LogFormat("{0}: {1}", chat.Player.Name, chat.Message);
    }

    #endregion
}
