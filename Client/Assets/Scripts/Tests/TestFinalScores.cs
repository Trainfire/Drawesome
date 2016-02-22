using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System.Linq;

public class TestFinalScores : MonoBehaviour
{
    public UiGameStateFinalScores View;

    Dictionary<PlayerData, GameScore> scoreCache = new Dictionary<PlayerData, GameScore>();
    List<PlayerData> players = new List<PlayerData>();

    void Awake()
    {
        View = GetComponent<UiGameStateFinalScores>();
        players = TestUtility.GetPlayerData();
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
        // Show values on UI
        View.Show(serverScores);
    }
}
