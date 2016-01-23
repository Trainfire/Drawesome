using System;

namespace Protocol
{
    public enum GameState
    {
        None,
        Setup,
        DrawingPhase,
    }

    [Serializable]
    public enum MessageType
    {
        None,
        Generic,
        PlayerConnect,
        ValidatePlayer,
        PlayerReady,
        ForceStartRound,
        SendChatFromPlayer,
        ServerUpdate,
    }
}
