using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Protocol;

public class UiGameStateFinalScores : UiBase
{
    public UiMostLikes MostLikes;

    public UiFinalScoreRow RowProtoype;
    public GameObject RowContainer;
    public Button StartNewGame;

    public float TimeBeforeFirst = 2f;
    public float TimeBetweenPlayers = 0.25f;
    public float TimeBeforeMostLikes = 2f;
    public float TimeBeforeShowButton = 4f;

    void Awake()
    {
        RowProtoype.gameObject.SetActive(false);
    }

    public void Show(GameFinalScores scores, GameFinalScores likes)
    {
        var animController = gameObject.GetOrAddComponent<UiAnimationController>();

        var views = new List<UiFinalScoreRow>();

        // Add rows
        foreach (var score in scores.Values.Take(3).ToList())
        {
            var instance = UiUtility.AddChild(RowContainer, RowProtoype, true);

            instance.PlayerName.text = string.Join(", ", scores.GetPlayerNames(score.Key));
            instance.Points.text = score.Key.ToString();

            views.Add(instance);
        }

        // Reverse order so it plays out from 3rd to 1st
        views.Reverse();

        // Show rows
        for (int i = 0; i < views.Count; i++)
        {
            views[i].Position.text = (views.Count - i).ToString();

            var delay = i == (views.Count - 1) ? TimeBeforeFirst : TimeBetweenPlayers;

            animController.AddDelay(delay);

            animController.AddAnim(new UiAnimationFade(views[i].gameObject, 0.2f, UiAnimationFade.FadeType.In), false);

            // If showing final player...
            if (i == (views.Count - 1))
            {
                views[i].Position.text = Strings.Winner;
                animController.AddAnim(new UiAnimationScale(views[i].gameObject, Vector3.one * 5, Vector3.one, 0.5f));
            }
        }

        // Show likes if any were made
        if (likes.Values.Any(x => x.Key != 0))
        {
            animController.AddDelay(TimeBeforeMostLikes);
            animController.AddAction("Show Most Likes", () => MostLikes.Show(likes));
        }

        animController.AddDelay(TimeBeforeShowButton);
        animController.AddAnim(new UiAnimationFade(StartNewGame.gameObject, 0.2f, UiAnimationFade.FadeType.In));

        animController.PlayAnimations();
    }
}
