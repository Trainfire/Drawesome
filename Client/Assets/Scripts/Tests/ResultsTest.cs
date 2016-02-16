using UnityEngine;
using System.Collections.Generic;
using Protocol;

public class ResultsTest : MonoBehaviour
{
    UiGameStateResults Results;

    void Awake()
    {
        Results = GetComponent<UiGameStateResults>();
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            Results.ShowAnswer(MakePlayerAnswer());
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            Results.ShowAnswer(MakeActualAnswer());
        }
    }

    AnswerData MakeActualAnswer()
    {
        var answer = new AnswerData();
        answer.Answer = "the answer goes here";
        answer.Points = 1000;

        var players = new List<PlayerData>();
        for (int i = 0; i < 8; i++)
        {
            var data = new PlayerData();
            data.Name = "Name[" + i + "]";
            answer.Choosers.Add(data);
        }

        answer.Type = GameAnswerType.ActualAnswer;

        return answer;
    }

    AnswerData MakePlayerAnswer()
    {
        var answer = new AnswerData();
        answer.Answer = "the answer goes here";
        answer.Points = 1000;

        var players = new List<PlayerData>();
        for (int i = 0; i < Random.Range(0, 8); i++)
        {
            var data = new PlayerData();
            data.Name = "Name[" + i + "]";
            answer.Choosers.Add(data);
        }

        answer.Type = GameAnswerType.Player;

        return answer;
    }
}
