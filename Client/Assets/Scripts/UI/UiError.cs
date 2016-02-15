using UnityEngine.UI;
using Protocol;

public class UiError : UiBase
{
    public Text Label;

    public void Show(GameAnswerError error)
    {
        base.Show();

        switch (error)
        {
            case GameAnswerError.None:
                Hide();
                break;
            case GameAnswerError.MatchesPrompt:
                Label.text = "That's the right answer! Quick! Think of something else!";
                break;
            case GameAnswerError.AlreadyExists:
                Label.text = "Somebody already entered that. Try again!";
                break;
            default:
                break;
        }
    }
}
