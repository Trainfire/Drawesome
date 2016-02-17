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
    public UiOrderedList List;

    public float TimeRevealTitle;
    public float TimeBetweenTitleAndDivider;
    public float TimeRevealDivider;
    public float TimeBeforeShowScores;
    public float TimeShowScores;
    public float TimeBetweenShowPointsAndUpdateList;
    public float TimeBeforeRearrangeList;

    Dictionary<PlayerData, UiResultRow> Views = new Dictionary<PlayerData, UiResultRow>();

    void Awake()
    {
        RowPrototype.gameObject.SetActive(false);
    }

    public void ShowScores(Dictionary<PlayerData, TestScores.ScoreWrapper> newScores)
    {
        RowPrototype.gameObject.SetActive(false);

        // Show titles
        AnimController.AddAnim(new UiAnimationFade(Title.gameObject, TimeRevealTitle, UiAnimationFade.FadeType.In));
        AnimController.AddDelay(TimeBetweenTitleAndDivider);

        // Show divider
        AnimController.AddAnim(new UiAnimationFade(Divider.gameObject, TimeRevealDivider, UiAnimationFade.FadeType.In));
        AnimController.AddDelay(TimeBeforeShowScores);

        float revealScores = TimeShowScores / newScores.Count;

        // Reveal current scores
        foreach (var score in newScores)
        {
            var view = AddOrGetView(score.Key, score.Value);
            var temp = score;
            AnimController.AddAction("Show Previous Scores", () =>
            {
                view.Score.text = temp.Value.PreviousScore.ToString();
                view.PointsEarned.text = "";
            });
            AnimController.AddAnim(new UiAnimationFade(view.gameObject, revealScores, UiAnimationFade.FadeType.In));
        }

        AnimController.AddDelay(1f);

        // Reveal points earned
        foreach (var score in newScores)
        {
            var view = AddOrGetView(score.Key, score.Value);
            var temp = score;
            AnimController.AddAction("Shows Points Earned", () =>
            {
                view.PointsEarned.text = string.Format("+{0}", temp.Value.PointsEarned);
            });
            AnimController.AddAnim(new UiAnimationFade(view.PointsEarned.gameObject, 0f, UiAnimationFade.FadeType.In));
        }

        AnimController.AddDelay(TimeBetweenShowPointsAndUpdateList);

        // Update scores
        foreach (var score in newScores)
        {
            var view = AddOrGetView(score.Key, score.Value);
            var temp = score;
            AnimController.AddAnim(new UiAnimationFade(view.PointsEarned.gameObject, 0.1f, UiAnimationFade.FadeType.Out));
            AnimController.AddAction("Update Scores", () =>
            {
                view.Score.text = temp.Value.CurrentScore.ToString();
            });
        }

        // Delay
        AnimController.AddDelay(TimeBeforeRearrangeList);

        // Reorder list
        AnimController.AddAction("Reorder List", () =>
        {
            List.OrderBy<TestScores.ScoreWrapper>((a, b) => a.CurrentScore.CompareTo(b.CurrentScore));
        });

        AnimController.PlayAnimations();
    }

    UiResultRow AddOrGetView(PlayerData player, TestScores.ScoreWrapper score)
    {
        if (!Views.ContainsKey(player))
        {
            var view = List.AddItem(score, RowPrototype);
            view.PlayerName.text = player.Name;
            Views.Add(player, view);
        }

        return Views[player];
    }
}
