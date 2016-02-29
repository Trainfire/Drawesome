using UnityEngine;
using System;   

public class PopupFactory : MonoBehaviour
{
    public UiPopup Popup;

    public Popup MakePopup(string message, Action onOkay = null)
    {
        var instance = UiUtility.AddChild(gameObject, Popup);

        var rect = (RectTransform)instance.transform;
        rect.sizeDelta = Vector2.one;

        // Bring to front
        instance.transform.SetAsLastSibling();

        return new Popup(instance, message, onOkay);
    }
}
