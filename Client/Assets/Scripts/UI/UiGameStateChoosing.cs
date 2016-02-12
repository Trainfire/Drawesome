using UnityEngine;
using System.Linq;

public class UiGameStateChoosing : UiGameState
{
    bool hasShownChoices = false;

    protected override void OnBegin()
    {
        base.OnBegin();
    }

    void Update()
    {
        if (!hasShownChoices)
        {
            var choices = Controller.Choices.Aggregate((current, next) => current + ", " + next);
            Debug.LogFormat("Display: {0}", choices);
            hasShownChoices = true;
        }
    }
}
