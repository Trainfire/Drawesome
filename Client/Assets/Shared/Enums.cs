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
    }
}
