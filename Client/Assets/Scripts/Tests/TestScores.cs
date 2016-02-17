using UnityEngine;
using System.Collections.Generic;
using Protocol;

public class TestScores : MonoBehaviour
{
    UiGameStateScores View;

    void Awake()
    {
        View = GetComponent<UiGameStateScores>();
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            var data = GetData();
            View.ShowScores(data);
        }
    }

    List<KeyValuePair<PlayerData, uint>> GetData()
    {
        var data = new List<KeyValuePair<PlayerData, uint>>();
        for (int i = 0; i < Random.Range(4, 8); i++)
        {
            var player = new PlayerData();
            player.Name = "Name[" + i + "]";

            var score = (uint)Random.Range(0, 2000);

            data.Add(new KeyValuePair<PlayerData, uint>(player, score));
        }

        return data;
    }
}
