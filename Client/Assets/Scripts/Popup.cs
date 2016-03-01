using UnityEngine;
using System.Collections;
using System;

public abstract class Popup
{
    UiPopupBase View { get; set; }

    public Popup(UiPopupBase view)
    {
        View = view;
        View.gameObject.SetActive(false);
    }

    public void Show()
    {
        View.Show();
    }
}

public class PopupMessage : Popup
{
    public PopupMessage(UiPopupMessage view, string message, Action onOkay = null) : base(view)
    {
        view.ButtonOkay.onClick.AddListener(() =>
        {
            if (onOkay != null)
                onOkay();

            view.Hide();
        });

        view.Message.text = message;
    }
}

public class PopupConfirm : Popup
{
    public PopupConfirm(UiPopupConfirm view, string message, Action onConfirm = null) : base(view)
    {
        view.ButtonConfirm.onClick.AddListener(() =>
        {
            if (onConfirm != null)
                onConfirm();

            view.Hide();
        });

        view.ButtonCancel.onClick.AddListener(() => view.Hide());

        view.Message.text = message;
    }
}

public class PopupInput : Popup
{
    public PopupInput(UiPopupInput view, string title, Action<string> onSubmit = null) : base(view)
    {
        view.Title.text = title;

        view.Okay.onClick.AddListener(() =>
        {
            if (onSubmit != null)
                onSubmit(view.Input.text);

            view.Hide();
        });

        view.Cancel.onClick.AddListener(() => view.Hide());
    }
}
