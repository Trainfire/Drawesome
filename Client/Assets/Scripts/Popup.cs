using UnityEngine;
using System.Collections;
using System;

public class Popup
{
    UiPopup View { get; set; }

    public Popup(UiPopup view, string message, Action onOkay = null)
    {
        View = view;

        View.Hide();

        // Assign callback
        View.ButtonOkay.onClick.AddListener(() =>
        {
            if (onOkay != null)
                onOkay();

            View.Hide();
        });

        View.Message.text = message;
    }

    public void Show()
    {
        View.Show();
    }
}
