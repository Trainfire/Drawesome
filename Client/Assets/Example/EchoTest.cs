using UnityEngine;
using System.Collections;
using System;
using WebSocketSharp;
using Protocol;

public class EchoTest : MonoBehaviour
{
    public bool IsReady;

    Action guiState;
    string playerName = "";

    void OnGUI()
    {
        GUILayout.BeginHorizontal();

        if (!Client.Instance.IsConnected())
        {
            guiState = OnDisconnected;
        }

        if (Client.Instance.IsConnected())
            guiState = OnConnected;

        guiState();

        GUILayout.EndHorizontal();
    }

    void OnDisconnected()
    {
        playerName = GUILayout.TextField(playerName, GUILayout.Width(400f));

        if (GUILayout.Button("Connect"))
        {
            Client.Instance.Connect(playerName);
        }
    }

    void OnConnected()
    {
        if (GUILayout.Button("Disconnect"))
        {
            Client.Instance.Disconnect();
        }

        if (GUILayout.Button("Request Rooms"))
            Client.Instance.RequestRooms();
    }
}
