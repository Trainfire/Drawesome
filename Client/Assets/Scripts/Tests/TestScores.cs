using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System;

public class TestScores : MonoBehaviour
{
    UiGameStateScores View;

    public class ScoreWrapper
    {
        public uint PreviousScore;
        public uint CurrentScore;

        public uint PointsEarned { get { return CurrentScore - PreviousScore; } }

        public override string ToString()
        {
            return CurrentScore.ToString();
        }
    }

    Dictionary<PlayerData, GameScore> scoreCache = new Dictionary<PlayerData, GameScore>();

    void Awake()
    {
        View = GetComponent<UiGameStateScores>();
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            HandleScores(TestUtility.GetPlayerScores());
        }
    }

    void HandleScores(Dictionary<PlayerData, ScoreData> serverScores)
    {
        foreach (var serverScore in serverScores)
        {
            if (!scoreCache.ContainsKey(serverScore.Key))
                scoreCache.Add(serverScore.Key, new GameScore(serverScore.Value));

            // Update score
            scoreCache[serverScore.Key].UpdateScore(serverScore.Value);
        }

        // Show values on UI
        View.ShowScores(scoreCache);
    }
}
