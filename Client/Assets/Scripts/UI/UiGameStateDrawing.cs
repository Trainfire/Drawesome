using UnityEngine;
using UnityEngine.UI;

public class UiGameStateDrawing : UiBase
{
    public Text Prompt;
    public Button Submit;

    public void SetPrompt(string prompt)
    {
        Prompt.text = prompt;
    }
}
