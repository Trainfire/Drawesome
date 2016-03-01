using UnityEngine;
using System;   

public class PopupFactory : MonoBehaviour
{
    public UiPopupMessage PopupMessage;
    public UiPopupInput PopupInput;
    public UiPopupConfirm PopupConfirm;

    T MakePopup<T>(T prototype) where T : UiPopupBase
    {
        var instance = UiUtility.AddChild(gameObject, prototype);

        var rect = (RectTransform)instance.transform;
        rect.sizeDelta = Vector2.one;

        // Bring to front
        instance.transform.SetAsLastSibling();

        return instance;
    }

    public Popup MakeMessagePopup(string message, Action onOkay = null)
    {
        return new PopupMessage(MakePopup(PopupMessage), message, onOkay);
    }

    public Popup MakeInputPopup(string title, Action<string> onSubmit = null)
    {
        return new PopupInput(MakePopup(PopupInput), title, onSubmit);
    }

    public Popup MakeConfirmationPopup(string title, Action onConfirm = null)
    {
        return new PopupConfirm(MakePopup(PopupConfirm), title, onConfirm);
    }
}
