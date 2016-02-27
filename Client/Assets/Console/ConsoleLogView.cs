using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ConsoleLogView : MonoBehaviour
{
    float TimeToLive { get; set; }
    float TimeStamp { get; set; }
    CanvasGroup Canvas { get; set; }

    const float FadeTime = 1f;

    public void SetText(string text, float timeToLive)
    {
        Canvas = gameObject.GetOrAddComponent<CanvasGroup>();

        GetComponent<Text>().text = text;
        TimeToLive = timeToLive;
        TimeStamp = Time.time;

        gameObject.SetActive(true);
    }

    void Update()
    {
        if (Time.time > TimeStamp + TimeToLive)
        {
            Canvas.alpha -= Time.deltaTime / FadeTime;

            if (Time.time > TimeStamp + TimeToLive + FadeTime)
                Destroy(gameObject);
        }
    }
}
