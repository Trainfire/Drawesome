using UnityEngine;
using UnityEngine.UI;

public class UiGameStateAnswering : UiGameState
{
    public InputField InputField;
    public Button Submit;

    protected override void OnBegin()
    {
        base.OnBegin();

        Submit.onClick.AddListener(() => Controller.SubmitAnswer(InputField.text));
    }

    protected override void OnEnd()
    {
        Submit.onClick.RemoveAllListeners();
    }
}
