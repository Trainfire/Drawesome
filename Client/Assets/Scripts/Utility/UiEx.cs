using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

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

    public static void Fade(this CanvasGroup canvas, float from, float to, float duration)
    {
        var mono =canvas.GetComponent<MonoBehaviour>();
        mono.StartCoroutine(_Fade(canvas, from, to, duration));
    }

    static IEnumerator _Fade(CanvasGroup canvas, float from, float to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            canvas.alpha = Mathf.Lerp(from, to, time / duration);
            time += Time.deltaTime;
            yield return 0;
        }
        canvas.alpha = to;
        yield return 0;
    }
}
