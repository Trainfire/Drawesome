using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System.Linq;

public class TestFinalScores : MonoBehaviour
{
    public UiGameStateFinalScores View;

    void Awake()
    {
        View = GetComponent<UiGameStateFinalScores>();
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            HandleScores(TestUtility.GetPlayerScores(3));
        }
    }

    void HandleScores(Dictionary<PlayerData, ScoreData> serverScores)
    {
        // Show values on UI
        View.Show(serverScores);
    }
}
