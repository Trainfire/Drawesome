using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

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
        var mono = canvas.GetComponent<MonoBehaviour>();
        mono.StartCoroutine(TweenFloat(canvas, from, to, duration, (value) =>
        {
            canvas.alpha = value;
        }));
    }

    public static void Scale(this CanvasGroup canvas, Vector2 from, Vector2 to, float duration)
    {
        var mono = canvas.GetComponent<MonoBehaviour>();
        mono.StartCoroutine(TweenVector(canvas, from, to, duration, (value) =>
        {
            canvas.transform.localScale = value;
        }));
    }

    static IEnumerator TweenVector(CanvasGroup canvas, Vector3 from, Vector3 to, float duration, Action<Vector3> onTween)
    {
        float time = 0f;
        Vector3 v = from;
        while (time < duration)
        {
            v = Vector3.Lerp(from, to, time / duration);
            time += Time.deltaTime;
            onTween(v);
            yield return v;
        }
        v = to;
        onTween(v);
        yield return v;
    }

    static IEnumerator TweenFloat(CanvasGroup canvas, float from, float to, float duration, Action<float> onTween)
    {
        float time = 0f;
        float v = from;
        while (time < duration)
        {
            v = Mathf.Lerp(from, to, time / duration);
            time += Time.deltaTime;
            onTween(v);
            yield return v;
        }
        v = to;
        onTween(v);
        yield return v;
    }
}
