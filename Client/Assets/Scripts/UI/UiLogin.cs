using UnityEngine;
using UnityEngine.UI;

public class UiLogin : UiMenu
{
    public InputField InputName;
    public Button Login;

    void Start()
    {
        Login.onClick.AddListener(() => Client.Instance.Connection.Connect(InputName.text));
    }
}
