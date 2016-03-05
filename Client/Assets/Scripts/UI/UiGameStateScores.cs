using UnityEngine;
using System.Collections;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;

public class UiGameStateScores : UiBase
{
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
    public float TimeBeforeLikes;

    Dictionary<PlayerData, UiResultRow> Views = new Dictionary<PlayerData, UiResultRow>();

    void Awake()
    {
        RowPrototype.gameObject.SetActive(false);
    }

    public void ShowScores(Dictionary<PlayerData, GameScore> newScores)
    {
        RowPrototype.gameObject.SetActive(false);

        // Show titles
        AnimController.AddAnim(new UiAnimationFade(Title.gameObject, TimeRevealTitle, UiAnimationFade.FadeType.In));
        AnimController.AddDelay(TimeBetweenTitleAndDivider);

        // Show divider
        AnimController.AddAnim(new UiAnimationFade(Divider.gameObject, TimeRevealDivider, UiAnimationFade.FadeType.In));
        AnimController.AddDelay(TimeBeforeShowScores);

        // Calculate how long it should take to reveal each player's score based on the number of players
        float revealScores = TimeShowScores / newScores.Count;

        // Set the previous scores
        foreach (var score in newScores)
        {
            var view = AddOrGetView(score.Key, score.Value);
            var temp = score;
            AnimController.AddAction("Show Previous Scores", () =>
            {
                view.Score.text = temp.Value.PreviousScore.ToString();
                view.PointsEarned.text = "";
                view.Likes.gameObject.SetActive(false);
            });
        }

        // Get all the player rows in order from top to bottom
        var views = List.GetComponentsInChildren<UiResultRow>().ToList();

        // Fade in each row
        views.ForEach(x => AnimController.AddAnim(new UiAnimationFade(x.gameObject, revealScores, UiAnimationFade.FadeType.In)));
        AnimController.AddDelay(1f);

        // Set points earned
        foreach (var score in newScores)
        {
            var view = AddOrGetView(score.Key, score.Value);
            var temp = score;
            AnimController.AddAction("Shows Points Earned", () =>
            {
                view.PointsEarned.text = string.Format("+{0}", temp.Value.PointsEarned);
            });
        }

        // Reveal points earned
        views.ForEach(x => AnimController.AddAnim(new UiAnimationFade(x.PointsEarned.gameObject, 0f, UiAnimationFade.FadeType.In)));
        AnimController.AddDelay(TimeBetweenShowPointsAndUpdateList);

        // Update scores
        foreach (var score in newScores)
        {
            var view = AddOrGetView(score.Key, score.Value);
            var temp = score;

            // Reveal player's answer underneath if they gave one
            view.Answer.text = temp.Value.CurrentScoreData.AnswerGiven != null ? temp.Value.CurrentScoreData.AnswerGiven.Answer : "";

            // Show the player's current score
            AnimController.AddAction("Update Scores", () =>
            {
                view.Score.text = temp.Value.CurrentScoreData.Score.ToString();
            });
        }

        // Fade in the points earned text displayed on right side as + 1234
        views.ForEach(x => AnimController.AddAnim(new UiAnimationFade(x.PointsEarned.gameObject, 0.1f, UiAnimationFade.FadeType.Out), false));
        AnimController.AddDelay(TimeBeforeRearrangeList);

        // Reorder list
        AnimController.AddAction("Reorder List", () =>
        {
            List.OrderBy<GameScore>((a, b) => a.CurrentScoreData.Score.CompareTo(b.CurrentScoreData.Score));
        });

        // Set likes
        AnimController.AddDelay(TimeBeforeLikes);

        foreach (var score in newScores)
        {
            if (score.Value.CurrentScoreData.AnswerGiven != null && score.Value.CurrentScoreData.AnswerGiven.Likes != 0)
            {
                var view = AddOrGetView(score.Key, score.Value);
                var temp = score;
                AnimController.AddAction("Update Likes", () =>
                {
                    view.Likes.gameObject.SetActive(true);
                    view.LikesEarned.text = "+" + temp.Value.CurrentScoreData.AnswerGiven.Likes.ToString();
                });
            }
        }

        // Fade in likes
        views.ForEach(x => AnimController.AddAnim(new UiAnimationFade(x.Likes, 0.1f, UiAnimationFade.FadeType.In), false));

        AnimController.PlayAnimations();
    }

    UiResultRow AddOrGetView(PlayerData player, GameScore score)
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
