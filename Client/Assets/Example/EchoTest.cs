using UnityEngine;
using System.Collections;
using System;
using WebSocketSharp;
using Protocol;

public class EchoTest : MonoBehaviour
{

    const string URI = "ws://127.0.0.1:8181/room";

    WebSocket webSocket;

    public bool IsReady;

    void Start()
    {
        webSocket = new WebSocket(URI);
        webSocket.Connect();
        webSocket.OnMessage += OnMessage;
    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log("Recieved message: " + e.Data);
    }

    void OnApplicationQuit()
    {
        webSocket.Close();
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Ready?"))
        {
            IsReady = !IsReady;
            var data = new  PlayerReadyMessage(IsReady);
            var json = JsonUtility.ToJson(data);
            Debug.Log(json);
            webSocket.Send(json);
        }

        if (GUILayout.Button("Start"))
        {
            IsReady = !IsReady;
            var data = new Message(MessageType.ForceStartRound);
            var json = JsonUtility.ToJson(data);
            webSocket.Send(json);
        }

        GUILayout.EndHorizontal();
    }
}
