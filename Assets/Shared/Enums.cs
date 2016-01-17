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
        PlayerConnect,
        PlayerReady,
        ForceStartRound,
        Generic,
    }
}
