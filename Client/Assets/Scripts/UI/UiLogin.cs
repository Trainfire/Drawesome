using UnityEngine;
using UnityEngine.UI;
using Protocol;

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
            var name = ValidateInput(InputName.text);
            Client.Connection.Connect(name);
        });

        InfoBox.Hide();

        client.MessageHandler.OnConnectionError += OnConnectionError;
    }

    void OnConnectionError(ServerMessage.SendConnectionError message)
    {
        if (message.Error == ConnectionError.None)
        {
            InfoBox.Hide();
        }
        else
        {
            InfoBox.Show(message.Error);
        }
    }

    string ValidateInput(string input)
    {
        input = input.Trim();
        input = input.Replace("\t", "");
        return input;
    }
}
