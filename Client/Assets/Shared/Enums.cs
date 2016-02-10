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
        Choosing,
        Results,
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
        ClientStartGame = 105,

        ServerUpdate = 200,
        ServerNotifyRoomError = 201,
        ServerRoomUpdate = 202,
        ServerSendRoomList = 203,
        ServerNotifyPlayerAction = 204,
        ServerConnectionSuccess = 205,

        GameClientSendImage = 300,
        GameClientSubmitAnswer = 301,
        GameClientSubmitChoice = 302,
        GameClientSubmitLike = 303,
        GameClientSkipPhase = 304,

        GameServerStateChange,
        GameServerSendImage,
        GameServerSendPrompt,
        GameServerSendChoices,
        GameServerSendResult,
    }
}
