using System;

namespace Protocol
{
    public enum MessageDataType
    {
        JSON,
        Binary,
    }

    public enum PlayerActionContext
    {
        Global,
        Room,
    }

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

    public enum ConnectionError
    {
        None,
        InvalidNameLength,
        MatchesExistingName,
        ProtocolMismatch,
    }

    public enum RoomNotice
    {
        None,
        InvalidPassword,
        RoomFull,
        RoomDoesNotExist,
        AlreadyInRoom,
        GameAlreadyStarted,
        MaxRoomsLimitReached
    }

    public enum RoomLeaveReason
    {
        Normal,
        Kicked,
    }

    public enum GameAction
    {
        Start,
        Restart,
        StartNewRound,
        FinishShowingResult,
        CancelStart,
        ForceStart,
    }

    public enum GamePlayerAction
    {
        None,
        DrawingSubmitted,
        AnswerSubmitted,
        ChoiceChosen,
    }

    public enum GameAnswerValidationResponse
    {
        None,
        MatchesPrompt,
        AlreadyExists,
        Empty,
    }

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

    public enum GameTransition
    {
        RoundBeginToDrawing,
        DrawingToAnswering,
        AnsweringToChoosing,
        ChoosingToResults,
        ResultsToScores,
        ScoresToAnswering,
    }

    public enum GameStateEndReason
    {
        Normal,
        TimerExpired,
        Skipped,
        GameEnded,
    }

    public enum GameAnswerType
    {
        Player,
        Decoy,
        ActualAnswer,
    }
}
