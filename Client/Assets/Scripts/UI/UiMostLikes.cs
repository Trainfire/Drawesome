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

    public void Show(GameFinalScores likes)
    {
        var mostLikes = likes.Values.Aggregate((current, next) => next.Key > current.Key ? next : current);

        var view = UiUtility.AddChild(Rows, RowPrototype, true);
        view.Name.text = string.Join(", ", likes.GetPlayerNames(mostLikes.Key));
        view.Points.text = mostLikes.Key.ToString();

        var animController = gameObject.GetOrAddComponent<UiAnimationController>();
        gameObject.SetActive(true);
        animController.AddAnim(new UiAnimationFade(gameObject, 0.25f, UiAnimationFade.FadeType.In));
        animController.PlayAnimations();
    }
}
