using System;

namespace Protocol
{
    [Serializable]
    public enum PlayerAction
    {
        None = 0,
        Connected = 1,
        Disconnected = 2,
        Kicked = 3,
        Joined = 4,
        Left = 5,
        PromotedToOwner = 6,
        Ready = 7,
        Unready = 8,
    }

    [Serializable]
    public enum RoomError
    {
        None,
        InvalidPassword,
        RoomFull,
        RoomDoesNotExist,
        AlreadyInRoom,
    }

    [Serializable]
    public enum GameState
    {
        RoundBegin,
        Drawing,
        Answering,
        ShowChoices,
        MakeChoices,
        RoundEnd,
    }

    [Serializable]
    public enum MessageType
    {
        None = 0,
        Log = 1,
        Chat = 2,

        ClientConnectionRequest = 100,
        ClientRequestRoomList = 101,
        ClientJoinRoom = 102,
        ClientLeaveRoom = 103,
        ClientCreateRoom = 104,

        ServerUpdate = 200,
        ServerNotifyRoomError = 201,
        ServerRoomUpdate = 202,
        ServerSendRoomList = 203,
        ServerNotifyPlayerAction = 204,
        ServerConnectionSuccess = 205,

        GameStateChange = 300,

        GameClientSubmitDrawing = 301,
        GameClientSubmitAnswer = 302,
        GameClientSubmitChoice = 303,

        GameServerShowChoices = 350,
    }
}
