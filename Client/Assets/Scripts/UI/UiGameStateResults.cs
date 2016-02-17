using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using System;

public class UiGameStateResults : UiBase
{
    public Text Answer;
    public CanvasGroup Divider;
    public Text Author;
    public Text Points;
    public Text ChosenBy;
    public UiResultRow RowPrototype;
    public GameObject Rows;
    public GameObject Result;

    public float TimeBeforeShowAnswer = 1f;
    public float TimeBeforeShowChosenBy = 0.2f;
    public float TimeBeforePlayersReveal = 0.2f;

    // The amount of time it should take to reveal all players
    public float TimeToRevealPlayers = 0.1f;

    public float TimeBeforeAuthorReveal = 0.2f;
    public float TimeBeforeScoreReveal = 0.5f;
    public float TimeBeforeShowNextResult = 2f;
    public float TimeBeforeActualAnswerReveal = 0.5f;
    public float TimeAfterActualAnswerReveal = 2f;

    public float TimeAuthorScale = 1f;
    public float TimeAuthorFade = 1f;
    public float TimeScoreScale = 1f;
    public float TimeScoreFade = 1f;
    public float TimeFadeAfterResult = 0.1f;

    public event Action OnFinishedShowingResult;

    List<GameObject> resultViews { get; set; }

    protected override void OnShow()
    {
        base.OnShow();
    }

    protected override void OnHide()
    {
        Clear();
    }

    void Clear()
    {
        if (resultViews != null)
            resultViews.ForEach(x => Destroy(x.gameObject));

        if (resultViews == null)
            resultViews = new List<GameObject>();

        Answer.gameObject.SetActive(false);
        RowPrototype.gameObject.SetActive(false);
        Author.gameObject.SetActive(false);
        Divider.gameObject.SetActive(false);
        ChosenBy.gameObject.SetActive(false);
        Points.gameObject.SetActive(false);
    }

    public void ShowAnswer(AnswerData result)
    {
        Clear();
        StartCoroutine(Animate(result));
    }

    // Kill me now!
    IEnumerator Animate(AnswerData result)
    {
        Result.GetOrAddComponent<CanvasGroup>().Fade(0f, 1f, 0.5f);

        yield return new WaitForSeconds(TimeBeforeShowAnswer);

        Answer.gameObject.SetActive(true);
        Answer.text = string.Format("{0}", result.Answer);

        yield return new WaitForSeconds(TimeBeforeShowChosenBy);

        Divider.gameObject.SetActive(true);
        Divider.Fade(0f, 1f, 0.1f);
        ChosenBy.gameObject.SetActive(true);
        ChosenBy.gameObject.GetOrAddComponent<CanvasGroup>().Fade(0f, 1f, 0.1f);

        yield return new WaitForSeconds(TimeBeforePlayersReveal);

        float revealTime = TimeToRevealPlayers / result.Choosers.Count;
        foreach (var player in result.Choosers)
        {
            yield return new WaitForSeconds(revealTime);
            var instance = UiUtility.AddChild(Rows, RowPrototype, true);
            instance.PlayerName.text = player.Name;
            instance.gameObject.GetOrAddComponent<CanvasGroup>().Scale(Vector3.one * 5, Vector3.one, 0.2f);
            resultViews.Add(instance.gameObject);
        }

        if (result.Type != GameAnswerType.ActualAnswer)
        {
            yield return new WaitForSeconds(TimeBeforeAuthorReveal);
        }
        else
        {
            yield return new WaitForSeconds(TimeBeforeActualAnswerReveal);
        }

        Author.gameObject.SetActive(true);
        Author.gameObject.GetOrAddComponent<CanvasGroup>().Fade(0f, 1f, TimeAuthorFade);
        Author.gameObject.GetOrAddComponent<CanvasGroup>().Scale(new Vector3(5f, 5f, 1f), Vector3.one, TimeAuthorScale);

        if (result.Type == GameAnswerType.Player)
        {
            Author.text = string.Format("{0}'s guess!", result.Author.Name);
            yield return new WaitForSeconds(TimeBeforeScoreReveal);
            Points.gameObject.SetActive(true);
            Points.text = string.Format("+{0}", result.Points.ToString());
            Points.gameObject.GetOrAddComponent<CanvasGroup>().Fade(0f, 1f, TimeScoreFade);
            Points.gameObject.GetOrAddComponent<CanvasGroup>().Scale(new Vector3(5f, 5f, 1f), Vector3.one, TimeScoreScale);
        }
        else if (result.Type == GameAnswerType.Decoy)
        {
            Author.text = string.Format("Decoy!");
            yield return new WaitForSeconds(TimeBeforeScoreReveal);
        }
        else
        {
            Author.text = string.Format("The actual answer!");
            yield return new WaitForSeconds(TimeBeforeScoreReveal);
        }

        if (result.Type != GameAnswerType.ActualAnswer)
        {
            yield return new WaitForSeconds(TimeBeforeShowNextResult);
        }
        else
        {
            yield return new WaitForSeconds(TimeAfterActualAnswerReveal);
        }

        Result.GetOrAddComponent<CanvasGroup>().Fade(1f, 0f, TimeFadeAfterResult);

        yield return new WaitForSeconds(TimeFadeAfterResult);

        if (OnFinishedShowingResult != null)
            OnFinishedShowingResult();

        yield return 0;
    }
}
