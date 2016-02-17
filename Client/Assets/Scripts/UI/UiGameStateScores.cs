using UnityEngine;
using System.Collections;
using Protocol;
using System;
using System.Collections.Generic;

public class UiGameStateScores : UiBase
{
    public UiAnimationController AnimController;

    public GameObject Title;
    public GameObject Divider;
    public UiResultRow RowPrototype;
    public GameObject Rows;

    public float TimeRevealTitle;
    public float TimeBetweenTitleAndDivider;
    public float TimeRevealDivider;
    public float TimeBeforeShowScores;
    public float TimeShowScores;
    public float TimeBeforeWinnerReveal;

    List<UiResultRow> resultViews = new List<UiResultRow>();

    List<KeyValuePair<PlayerData, uint>> cachedScores = new List<KeyValuePair<PlayerData, uint>>();

    public void ShowScores(List<KeyValuePair<PlayerData, uint>> scores)
    {
        RowPrototype.gameObject.SetActive(false);

        AnimController.AddAnim(new UiAnimationFade(Title.gameObject, TimeRevealTitle, UiAnimationFade.FadeType.In));
        AnimController.AddDelay(TimeBetweenTitleAndDivider);
        AnimController.AddAnim(new UiAnimationFade(Divider.gameObject, TimeRevealDivider, UiAnimationFade.FadeType.In));
        AnimController.AddDelay(TimeBeforeShowScores);

        float revealScores = TimeShowScores / scores.Count;

        foreach (var data in scores)
        {
            var instance = UiUtility.AddChild(Rows, RowPrototype, true);

            instance.PlayerName.text = data.Key.Name;
            instance.Score.text = data.Value.ToString();

            var lastScore = 0f;
            lastScore = cachedScores.Find(x => x.Key == data.Key).Value;
            if (Mathf.Approximately(lastScore, 0f))
            {
                instance.PointsEarned.text = data.Value.ToString();
            }
            else
            {
                instance.PointsEarned.text = (data.Value - lastScore).ToString();
            }

            AnimController.AddAnim(new UiAnimationFade(instance.gameObject, revealScores, UiAnimationFade.FadeType.In));
        }

        AnimController.PlayAnimations();

        cachedScores = scores;
    }

    List<UiResultRow> Players()
    {
        if (resultViews.Count == 0)
        {
            var instance = UiUtility.AddChild(Rows, RowPrototype, true);
            resultViews.Add(instance);
        }
        return resultViews;
    }

    IEnumerator Animate(List<KeyValuePair<PlayerData, uint>> scores)
    {
       
        yield return new WaitForSeconds(TimeBeforeWinnerReveal);

        // TODO
        AnimController.ClearQueue();
    }

    protected override void OnHide()
    {
        base.OnHide();
        resultViews.ForEach(x => Destroy(x.gameObject));
        resultViews.Clear();
    }
}
