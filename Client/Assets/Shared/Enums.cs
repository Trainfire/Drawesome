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
    public enum ConnectionError
    {
        None,
        InvalidNameLength,
        MatchesExistingName,
    }

    [Serializable]
    public enum RoomNotice
    {
        None,
        InvalidPassword,
        RoomFull,
        RoomDoesNotExist,
        AlreadyInRoom,
        GameAlreadyStarted,
    }

    [Serializable]
    public enum RoomLeaveReason
    {
        Normal,
        Kicked,
    }

    [Serializable]
    public enum GameAction
    {
        Start,
        Restart,
        StartNewRound,
        FinishShowingResult,
        CancelStart,
    }

    [Serializable]
    public enum GamePlayerAction
    {
        None,
        DrawingSubmitted,
        AnswerSubmitted,
        ChoiceChosen,
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
        FinalScores,
        GameOver,
    }

    [Serializable]
    public enum GameTransition
    {
        RoundBeginToDrawing,
        DrawingToAnswering,
        AnsweringToChoosing,
        ChoosingToResults,
        ResultsToScores,
        ScoresToAnswering,
    }

    [Serializable]
    public enum GameStateEndReason
    {
        Normal,
        TimerExpired,
        Skipped,
        GameEnded,
    }

    [Serializable]
    public enum GameAnswerType
    {
        Player,
        Decoy,
        ActualAnswer,
    }
}
