using UnityEngine;
using System.Collections.Generic;
using Protocol;

public class TestResults : Test
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

    public override void PerformTest()
    {
        base.PerformTest();
        gameObject.SetActive(true);
        StopAllCoroutines();
        Results.ShowAnswer(MakePlayerAnswer());
    }

    AnswerData MakeActualAnswer()
    {
        var answer = new AnswerData();
        answer.Answer = "the answer goes here";
        answer.Points = 1000;

        answer.Choosers = TestUtility.GetPlayerData();

        answer.Type = GameAnswerType.ActualAnswer;

        return answer;
    }

    AnswerData MakePlayerAnswer()
    {
        var answer = new AnswerData();
        answer.Answer = "the answer goes here";
        answer.Points = 1000;

        answer.Choosers = TestUtility.GetPlayerData();

        answer.Type = GameAnswerType.Player;

        return answer;
    }
}
