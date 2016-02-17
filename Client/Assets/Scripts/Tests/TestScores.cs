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

    Dictionary<PlayerData, ScoreWrapper> scoreCache = new Dictionary<PlayerData, ScoreWrapper>();
    List<PlayerData> players = new List<PlayerData>();

    void Awake()
    {
        View = GetComponent<UiGameStateScores>();
        players = TestUtility.GetPlayerData();
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            HandleScores(TestUtility.GetPlayerScores());
        }
    }

    void HandleScores(Dictionary<PlayerData, uint> serverScores)
    {
        foreach (var serverScore in serverScores)
        {
            if (!scoreCache.ContainsKey(serverScore.Key))
                scoreCache.Add(serverScore.Key, new ScoreWrapper());

            // Cache previous score
            scoreCache[serverScore.Key].PreviousScore = scoreCache[serverScore.Key].CurrentScore;

            Debug.LogFormat("{0}'s previous score is {1}", serverScore.Key.Name, scoreCache[serverScore.Key].PreviousScore);

            // Set current score
            scoreCache[serverScore.Key].CurrentScore = serverScore.Value;
        }

        // Show values on UI
        //View.ShowScores(scoreCache);
    }
}
