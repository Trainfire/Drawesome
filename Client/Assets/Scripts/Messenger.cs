using UnityEngine;
using Protocol;

public class Messenger
{
    Connection Connection { get; set; }

    public Messenger(Connection connection)
    {
        Connection = connection;
    }

    public void CreateRoom(string password = "")
    {
        Connection.SendMessage(new ClientMessage.CreateRoom(Connection.Data, password));
    }

    public void JoinRoom(string roomId, string password = "")
    {
        Debug.LogFormat("Attempt to join room {0}", roomId);
        Connection.SendMessage(new ClientMessage.JoinRoom(Connection.Data, roomId, password));
    }

    public void LeaveRoom()
    {
        Connection.SendMessage(new ClientMessage.LeaveRoom(Connection.Data));
    }

    public void RequestRooms()
    {
        Connection.SendMessage(new ClientMessage.RequestRoomList(Connection.Data));
    }

    public void Say(string message)
    {
        Connection.SendMessage(new SharedMessage.Chat(Connection.Data, message));
    }
}
