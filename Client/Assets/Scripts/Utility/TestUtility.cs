using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

public static class TestUtility
{
    /// <summary>
    /// Returns a list of PlayerData between 4 and 8 players.
    /// </summary>
    /// <returns></returns>
    public static List<PlayerData> GetPlayerData()
    {
        var data = new List<PlayerData>();
        for (int i = 0; i < Random.Range(4, 8); i++)
        {
            var player = new PlayerData();
            player.Name =  i + " Name";
            data.Add(player);
        }

        return data;
    }

    public static string GetRandomName()
    {
        var names = new List<string>()
        {
            "Aaron",
            "Billy",
            "Charlie",
            "David",
            "Poopy Joe",
            "Ronald Reagan",
            "u wot m8",
            "badman",
            "well good",
            "Aaron",
            "Billy",
            "Charlie",
            "David",
            "Poopy Joe",
            "Ronald Reagan",
            "u wot m8",
            "badman",
            "well good",
        };

        var rnd = new System.Random().Next(0, names.Count);
        Debug.Log(rnd);
        return names[rnd];
    }
}
