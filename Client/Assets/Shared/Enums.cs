using System;

namespace Protocol
{
    [Serializable]
    public enum PlayerAction
    {
        None,
        Connected,
        Disconnected,
        Kicked,
        Joined,
        Left,
        PromotedToOwner,
        Ready,
        Unready,
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
    public enum GameAction
    {
        Start,
        Restart,
        StartNewRound,
    }

    [Serializable]
    public enum GameAnswerError
    {
        None,
        MatchesPrompt,
        AlreadyExists,
    }

    [Serializable]
    public enum GameState
    {
        PreGame,
        RoundBegin,
        Drawing,
        Answering,
        Choosing,
        Results,
        Scores,
        RoundEnd,
    }
}
