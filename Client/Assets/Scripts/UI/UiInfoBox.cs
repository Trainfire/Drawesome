using UnityEngine.UI;
using Protocol;

public class UiInfoBox : UiBase
{
    public Text Label;

    public void Show(ConnectionError error)
    {
        base.Show();

        switch(error)
        {
            case ConnectionError.InvalidNameLength:
                Label.text = Strings.NameCharacterLimit;
                break;
            case ConnectionError.MatchesExistingName:
                Label.text = Strings.NameMatchesExisting;
                break;
        }
    }

    public void Show(GameAnswerValidationResponse error)
    {
        base.Show();

        switch (error)
        {
            case GameAnswerValidationResponse.None:
                Hide();
                break;
            case GameAnswerValidationResponse.MatchesPrompt:
                Label.text = Strings.AnswerMatchesPrompt;
                break;
            case GameAnswerValidationResponse.AlreadyExists:
                Label.text = Strings.AnswerMatchesExisting;
                break;
            case GameAnswerValidationResponse.Empty:
                Label.text = Strings.AnswerEmpty;
                break;
            default:
                break;
        }
    }

    public void Show(string message, params string[] args)
    {
        base.Show();
        Label.text = string.Format(message, args);
    }
}
