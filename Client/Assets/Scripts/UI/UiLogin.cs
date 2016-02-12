using UnityEngine;
using UnityEngine.UI;

public class UiLogin : UiBase
{
    public InputField InputName;
    public Button Login;

    public override void Initialise(Client client)
    {
        base.Initialise(client);

        Login.onClick.AddListener(() => Client.Connection.Connect(InputName.text));
    }
}
