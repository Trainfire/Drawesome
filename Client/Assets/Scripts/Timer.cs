using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    public UiTimer View;

    float Time { get; set; }

    public void Show()
    {
        View.gameObject.SetActive(true);
    }

    public void Hide()
    {
        View.gameObject.SetActive(false);
    }

    public void SetTime(float time)
    {
        Time = time;
    }

    void Update()
    {
        View.Fill.fillAmount = 60f / Time;
        View.Label.text = Time.ToString();
    }
}
