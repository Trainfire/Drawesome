using UnityEngine;
using System;
using WebSocketSharp;
using Protocol;
using System.Collections.Generic;

public class Client : Singleton<Client>
{
    public string ID { get; private set; }
    public string PlayerName { get; private set; }

    public List<ProtocolPlayer> Players;

    const string URI = "ws://127.0.0.1:8181/room";

    WebSocket webSocket;
    MessageHandler messageHandler = new MessageHandler();

    protected override void Awake()
    {
        base.Awake();
        webSocket = new WebSocket(URI);

        messageHandler.OnGeneric += OnGeneric;
        messageHandler.OnServerCompleteConnectionRequest += OnServerCompleteConnectionRequest;
        messageHandler.OnServerUpdate += OnServerUpdate;
        messageHandler.OnRecieveRoomList += OnRecieveRoomList;
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

    public void RequestRooms()
    {
        SendMessage(new ClientMessage.RequestRoomList(ID));
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

    }

    void OnMessage(object sender, MessageEventArgs e)
    {
        Debug.LogFormat("Recieved message: {0}", e.Data);
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

    void OnGeneric(Message message)
    {
        Debug.Log(message.LogMessage);
    }

    void OnRecieveRoomList(ServerMessage.RoomList message)
    {
        foreach (var room in message.Rooms)
        {
            Debug.LogFormat("Recieved room with ID {0}", room.ID);
        }
    }

    void OnServerCompleteConnectionRequest(ServerMessage.ConnectionSuccess message)
    {
        ID = message.ID;
        Debug.Log("Recieved ID: " + ID);
        var playerConnectMessage = new ClientMessage.RequestConnection(ID, PlayerName);
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

    #endregion
}
