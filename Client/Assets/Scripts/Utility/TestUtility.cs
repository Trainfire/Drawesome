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
    public static List<PlayerData> GetPlayerData(int amount = 8)
    {
        var data = new List<PlayerData>();
        for (int i = 0; i < Random.Range(1, amount); i++)
        {
            var player = new PlayerData();
            player.Name =  i + " Name";
            data.Add(player);
        }

        return data;
    }

    /// <summary>
    /// Returns score data for 8 players. Each score has a random value between 0 and 2000.
    /// </summary>
    /// <returns></returns>
    public static Dictionary<PlayerData, ScoreData> GetPlayerScores(int amount = 8)
    {
        var data = new Dictionary<PlayerData, ScoreData>();
        for (int i = 0; i < amount; i++)
        {
            var player = new PlayerData();
            player.ID = i.ToString();
            player.Name = "Player " + i;

            var score = (uint)Random.Range(0, 2000);
            var scoreData = new ScoreData(score, (uint)Random.Range(0, 4), new AnswerData(Random.Range(0, 1000).ToString()));
            scoreData.AnswerGiven.Likes = Random.Range(0, 5);
            data.Add(player, scoreData);
        }
        return data;
    }
}
