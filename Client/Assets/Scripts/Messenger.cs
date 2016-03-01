using UnityEngine;
using Protocol;
using System;

public class Messenger
{
    Connection Connection { get; set; }

    public event EventHandler OnLeaveRoom;

    public Messenger(Connection connection)
    {
        Connection = connection;
    }

    public void CreateRoom(string password = "")
    {
        Connection.SendMessage(new ClientMessage.CreateRoom(Connection.Player, password));
    }

    public void JoinRoom(string roomId, string password = "")
    {
        Debug.LogFormat("Attempt to join room {0}", roomId);
        Connection.SendMessage(new ClientMessage.JoinRoom(Connection.Player, roomId, password));
    }

    public void LeaveRoom()
    {
        Connection.SendMessage(new ClientMessage.LeaveRoom(Connection.Player));

        if (OnLeaveRoom != null)
            OnLeaveRoom(this, null);
    }

    public void RequestRooms()
    {
        Connection.SendMessage(new ClientMessage.RequestRoomList(Connection.Player));
    }

    public void Say(string message)
    {
        Connection.SendMessage(new ClientMessage.SendChat(message));
    }

    public void RequestAdmin(string password)
    {
        Connection.SendMessage(new ClientMessage.RequestAdmin(password));
    }

    #region Game

    public void StartGame()
    {
        Connection.SendMessage(new ClientMessage.Game.SendAction(Connection.Player, GameAction.Start));
    }

    public void ForceStartGame()
    {
        Connection.SendMessage(new ClientMessage.Game.SendAction(Connection.Player, GameAction.ForceStart));
    }

    public void CancelGameStart()
    {
        Connection.SendMessage(new ClientMessage.Game.SendAction(Connection.Player, GameAction.CancelStart));
    }

    public void RestartGame()
    {
        Connection.SendMessage(new ClientMessage.Game.SendAction(Connection.Player, GameAction.Restart));
    }

    public void StartNewRound()
    {
        Connection.SendMessage(new ClientMessage.Game.SendAction(Connection.Player, GameAction.StartNewRound));
    }

    public void StartNewGame()
    {
        Connection.SendMessage(new ClientMessage.Game.SendAction(Connection.Player, GameAction.Restart));
    }

    public void FinishShowingResult()
    {
        Connection.SendMessage(new ClientMessage.Game.SendAction(Connection.Player, GameAction.FinishShowingResult));
    }

    public void SendImage(byte[] image)
    {
        Connection.SendMessage(new ClientMessage.Game.SendImage(image));
    }

    public void SubmitAnswer(string answer)
    {
        Connection.SendMessage(new ClientMessage.Game.SubmitAnswer(answer));
    }

    public void SubmitChosenAnswer(string choice)
    {
        Connection.SendMessage(new ClientMessage.Game.SubmitChoice(choice));
    }

    public void SubmitLike(string answer)
    {
        Connection.SendMessage(new ClientMessage.Game.LikeAnswer(answer));
    }

    public void SkipPhase()
    {
        Connection.SendMessage(new ClientMessage.Game.SkipPhase());
    }

    #endregion
}
