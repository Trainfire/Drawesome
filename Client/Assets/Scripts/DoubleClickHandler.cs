using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DoubleClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Action OnDoubleClick;

    float ClickTimeStamp { get; set; }
    float DoubleClickTime = 0.2f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time < ClickTimeStamp + DoubleClickTime)
        {
            if (OnDoubleClick != null)
                OnDoubleClick();
        }

        ClickTimeStamp = Time.time;
    }
}
