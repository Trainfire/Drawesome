using UnityEngine;
using UnityEngine.UI;

public class UiGameStateDrawing : UiGameState
{
    public DrawingCanvas Canvas;
    public Text Prompt;
    public Button Submit;

    public void SetPrompt(string prompt)
    {
        Prompt.text = prompt;
    }
}
