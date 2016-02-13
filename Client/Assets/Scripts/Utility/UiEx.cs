using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class UiEx
{
    public static void Focus(this InputField inputField)
    {
        // Hack to keep focus on inputfield after submitting a command. Classic Unity.
        EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
        inputField.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    public static void ScrollToBottom(this ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, 0);
    }
}
