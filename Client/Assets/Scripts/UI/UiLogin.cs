using UnityEngine;
using UnityEngine.UI;

public class UiLogin : UiBase
{
    public InputField InputName;
    public UiInfoBox InfoBox;
    public Button Login;

    enum ValidationResult
    {
        Success,
        InvalidCharacterLength,
    }

    public override void Initialise(Client client)
    {
        base.Initialise(client);

        Login.onClick.AddListener(() =>
        {
            switch (IsInputValid(InputName.text))
            {
                case ValidationResult.Success:
                    InfoBox.Hide();
                    Client.Connection.Connect(InputName.text);
                    break;
                case ValidationResult.InvalidCharacterLength:
                    InfoBox.Show(string.Format(Strings.CharacterLimit, SettingsLoader.Settings.NameMinChars, SettingsLoader.Settings.NameMinChars));
                    break;
                default:
                    break;
            }
        });

        InfoBox.Hide();
    }

    ValidationResult IsInputValid(string input)
    {
        input = input.Trim();
        input = input.Replace("\t", "");

        if (input.Length > SettingsLoader.Settings.NameMaxChars)
            return ValidationResult.InvalidCharacterLength;

        if (input.Length < SettingsLoader.Settings.NameMinChars)
            return ValidationResult.InvalidCharacterLength;

        return ValidationResult.Success;
    }
}
