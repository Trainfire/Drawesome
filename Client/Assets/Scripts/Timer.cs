using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    public UiTimer View;

    public void Show()
    {
        View.gameObject.SetActive(true);
    }

    public void Hide()
    {
        View.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    public void SetTime(float time)
    {
        View.Fill.fillAmount = 60f / time;
        View.Label.text = time.ToString("F2");
    }
}
