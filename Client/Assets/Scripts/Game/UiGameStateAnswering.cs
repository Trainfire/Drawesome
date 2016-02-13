using UnityEngine;
using UnityEngine.UI;

public class UiGameStateAnswering : UiGameState
{
    public GameObject ErrorContainer;
    public Text ErrorLabel;
    public UiDrawingCanvas Canvas;
    public InputField InputField;
    public Button Submit;

    public override void RemoveAllListeners()
    {
        InputField.onEndEdit.RemoveAllListeners();
        Submit.onClick.RemoveAllListeners();
    }
}
