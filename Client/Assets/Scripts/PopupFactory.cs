using UnityEngine;
using System.Collections;
using System;   

public class PopupFactory : Singleton<PopupFactory>
{
    public UiPopup Popup;

    public Popup MakePopup(string message, Action onOkay = null)
    {
        var instance = UiUtility.AddChild<UiPopup>(gameObject, Popup);
        instance.transform.position = instance.transform.position + new Vector3(0, 0, 1f);
        return new Popup(instance, message, onOkay);
    }
}
