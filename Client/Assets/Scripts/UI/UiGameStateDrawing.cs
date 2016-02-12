using UnityEngine;
using UnityEngine.UI;

public class UiGameStateDrawing : UiGameState
{
    public DrawingCanvas Canvas;
    public Text Prompt;
    public Button Submit;

    protected override void OnBegin()
    {
        Submit.onClick.AddListener(() => Controller.SubmitDrawing(null));
    }

    void Update()
    {
        Prompt.text = Controller.Prompt;
    }

    protected override void OnEnd()
    {
        Submit.onClick.RemoveAllListeners();
    }
}
