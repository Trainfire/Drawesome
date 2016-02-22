using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Protocol;

public class UiGameStateFinalScores : UiBase
{
    public UiFinalScoreRow RowProtoype;
    public GameObject RowContainer;

    public float TimeBeforeFirst = 2f;
    public float TimeBetweenPlayers = 0.25f;

    void Awake()
    {
        RowProtoype.gameObject.SetActive(false);
    }

    public void Show(Dictionary<PlayerData, ScoreData> scoreData)
    {
        var animController = gameObject.GetOrAddComponent<UiAnimationController>();

        var views = new List<UiFinalScoreRow>();

        // Add rows
        foreach (var score in scoreData)
        {
            var instance = UiUtility.AddChild(RowContainer, RowProtoype, true);

            instance.PlayerName.text = score.Key.Name;
            instance.Points.text = score.Value.CurrentScore.ToString();

            views.Add(instance);
        }

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

            }
        }

        animController.PlayAnimations();
    }

    public class MockData
    {
        public string Name;
        public uint Points;

        public MockData(string name, uint points)
        {
            Name = name;
            Points = points;
        }
    }
}
