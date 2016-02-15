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
        GameAlreadyStarted,
    }

    [Serializable]
    public enum GameAction
    {
        Start,
        Restart,
        StartNewRound,
    }

    [Serializable]
    public enum GameAnswerValidationResponse
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
        GameOver,
    }
}
