using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;

public class UiGameStateResults : UiBase
{
    public Text Answer;
    public Text Author;
    public Text ChosenBy;
    public UiResultRow RowPrototype;
    public GameObject Rows;

    public float TimeBeforeShowAnswer = 1f;
    public float TimeBeforeShowChosenBy = 0.2f;
    public float TimeBeforePlayersReveal = 0.2f;
    public float TimeBetweenIndividualPlayerReveal = 0.1f;
    public float TimeBeforeAuthorReveal = 0.2f;
    public float TimeBeforeScoreReveal = 0.5f;

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
        ChosenBy.gameObject.SetActive(false);
    }

    public void ShowResult(ResultData result)
    {
        Clear();
        StartCoroutine(Animate(result));
    }

    IEnumerator Animate(ResultData result)
    {
        yield return new WaitForSeconds(TimeBeforeShowAnswer);

        Answer.gameObject.SetActive(true);
        Answer.text = result.Answer;

        yield return new WaitForSeconds(TimeBeforeShowChosenBy);

        ChosenBy.gameObject.SetActive(true);

        yield return new WaitForSeconds(TimeBeforePlayersReveal);

        foreach (var player in result.Players)
        {
            yield return new WaitForSeconds(TimeBetweenIndividualPlayerReveal);
            var instance = UiUtility.AddChild(Rows, RowPrototype, true);
            instance.PlayerName.text = player.Name;
            resultViews.Add(instance.gameObject);
        }

        yield return new WaitForSeconds(TimeBeforeAuthorReveal);

        Author.gameObject.SetActive(true);
        Author.text = string.Format("{0}'s guess!", result.Author.Name);

        yield return new WaitForSeconds(TimeBeforeScoreReveal);

        Author.text += string.Format(" +{0}", result.Points);

        yield return 0;
    }
}
