using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Protocol;

public class UiMostLikes : UiBase
{
    public GameObject Rows;
    public UiFinalScoreLikesRow RowPrototype;

    void Awake()
    {
        RowPrototype.gameObject.SetActive(false);
    }

    public void Show(Dictionary<PlayerData, ScoreData> scoreData)
    {
        var mostLikes = scoreData.Aggregate((current, next) => next.Value.Likes > current.Value.Likes ? next : current);

        var view = UiUtility.AddChild(Rows, RowPrototype, true);
        view.Name.text = mostLikes.Key.Name;
        view.Points.text = mostLikes.Value.Likes.ToString();

        var animController = gameObject.GetOrAddComponent<UiAnimationController>();
        gameObject.SetActive(true);
        animController.AddAnim(new UiAnimationFade(gameObject, 0.25f, UiAnimationFade.FadeType.In));
        animController.PlayAnimations();
    }
}
