using UnityEngine;
using System.Collections;
using System;
using WebSocketSharp;
using Protocol;

public class UserInterface : MonoBehaviour
{
    Action guiState;
    string playerName = "";
    string password = "";

    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        if (!Client.Instance.IsConnected())
        {
            guiState = OnDisconnected;
        }

        if (Client.Instance.IsConnected())
            guiState = OnConnected;

        guiState();

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
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
        // Top row
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Disconnect"))
        {
            Client.Instance.Disconnect();
        }

        if (GUILayout.Button("Request Rooms"))
            Client.Instance.RequestRooms();

        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();

        foreach (var room in Client.Instance.Rooms)
        {
            if (GUILayout.Button("Join room " + room.ID))
                Client.Instance.JoinRoom(room.ID);
        }

        GUILayout.EndVertical();

        if (GUILayout.Button("Leave Room"))
            Client.Instance.LeaveRoom();

        password = GUILayout.TextField(password, GUILayout.Width(400f));

        if (GUILayout.Button("Create Room"))
            Client.Instance.CreateRoom(password);
    }
}
