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
            guiState = OnConnected;
        }

        if (Client.Instance.IsConnected())
            guiState = OnDisconnected;

        guiState();

        GUILayout.EndHorizontal();
    }

    void OnConnected()
    {
        playerName = GUILayout.TextField(playerName, GUILayout.Width(400f));

        if (GUILayout.Button("Connect"))
        {
            Client.Instance.Connect(playerName);
        }
    }

    void OnDisconnected()
    {
        if (GUILayout.Button("Disconnect"))
        {
            Client.Instance.Disconnect();
        }
    }
}
