using UnityEngine;
using System;

public class UserInterface : MonoBehaviour
{
    public UiLogin ViewLogin;
    public UiBrowser ViewBrowser;

    UiMenu viewCurrent;

    void Start()
    {
        viewCurrent = ViewLogin;
        ViewLogin.OnShow();

        ViewBrowser.gameObject.SetActive(false);

        Client.Instance.OnConnect += OnConnect;
        Client.Instance.OnDisconnect += OnDisconnect;
    }

    private void OnDisconnect(object sender, EventArgs e)
    {
        ChangeMenu(ViewLogin);
    }

    void OnConnect(object sender, EventArgs e)
    {
        ChangeMenu(ViewBrowser);
    }

    void ChangeMenu(UiMenu newMenu)
    {
        viewCurrent.OnHide();
        newMenu.OnShow();
    }
}
