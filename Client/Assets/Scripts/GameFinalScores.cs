using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protocol;

/// <summary>
/// This class takes score data recieved from the server and groups them into tiers.
/// If two or more players have an identical score, they will be placed in the same tier.
/// </summary>
public class GameFinalScores
{
    Dictionary<uint, List<PlayerData>> values = new Dictionary<uint, List<PlayerData>>();
    public Dictionary<uint, List<PlayerData>> Values { get { return values; } }

    public GameFinalScores(Dictionary<PlayerData, ScoreData> serverScores, Func<ScoreData, uint> selector)
    {
        foreach (var score in serverScores)
        {
            var key = selector(score.Value);

            if (values.ContainsKey(key))
            {
                // If this score already exists, add this player to the same tier
                values[key].Add(score.Key);
            }
            else
            {
                // Otherwise, add the score and the player
                values.Add(key, new List<PlayerData>());
                values[key].Add(score.Key);
            }
        }
    }

    public string[] GetPlayerNames(uint key)
    {
        if (values.ContainsKey(key))
        {
            return values[key].Select(x => x.Name).ToArray();
        }
        return new string[0];
    }
}
