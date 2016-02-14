using UnityEngine;
using System.Collections;
using Protocol;
using System;
using System.Collections.Generic;

public class UiGameStateScores : UiBase
{
    public UiResultRow RowPrototype;
    public GameObject Rows;

    public float TimeBeforeShowScores;
    public float TimeBeforeIndividualScores;
    public float TimeBeforeWinnerReveal;

    List<GameObject> resultViews = new List<GameObject>();

    public void ShowScores(List<KeyValuePair<PlayerData, uint>> scores)
    {
        StartCoroutine(Animate(scores));
    }

    IEnumerator Animate(List<KeyValuePair<PlayerData, uint>> scores)
    {
        yield return new WaitForSeconds(TimeBeforeShowScores);

        foreach (var data in scores)
        {
            yield return new WaitForSeconds(TimeBeforeIndividualScores);
            var instance = UiUtility.AddChild(Rows, RowPrototype, true);
            instance.PlayerName.text = data.Key.Name;
            instance.Score.text = data.Value.ToString();
            resultViews.Add(instance.gameObject);
        }

        yield return new WaitForSeconds(TimeBeforeWinnerReveal);

        // TODO
    }
}
