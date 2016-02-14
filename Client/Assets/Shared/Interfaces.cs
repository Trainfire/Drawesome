using System;
using System.Collections.Generic;
using Protocol;

namespace Protocol
{
    public interface IScores
    {
        Dictionary<PlayerData, uint> PlayerScores { get; }
    }
}
