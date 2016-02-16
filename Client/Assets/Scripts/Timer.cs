using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    public UiTimer View;
    public float LerpTime = 0.8f;

    float Duration { get; set; }
    float CurrentTime { get; set; }
    bool shouldLerp = false;

    void Awake()
    {
        Duration = 30f;
        CurrentTime = Duration;
    }

    public void Show()
    {
        View.gameObject.SetActive(true);
    }

    public void Hide()
    {
        View.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    public void SetDuration(float duration)
    {
        Duration = duration;
        CurrentTime = duration;
        View.Fill.fillAmount = 1f;
        shouldLerp = false;
    }

    public void SetTime(float currentTime)
    {
        CurrentTime = currentTime;
        shouldLerp = true;
    }

    void Update()
    {
        if (shouldLerp)
            View.Fill.fillAmount = Mathf.Lerp(View.Fill.fillAmount, CalculateFill(), LerpTime);

        View.Label.text = string.Format("{0}s", Mathf.Floor(CurrentTime).ToString());
    }

    float CalculateFill()
    {
        return Mathf.Clamp01(CurrentTime / Duration);
    }
}
