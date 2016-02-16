using UnityEngine;
using System.Collections;

public class TimerTest : MonoBehaviour
{
    public UiTimer View;
    public Timer Timer;

    public float Time = 30f;

    void Start()
    {
        Timer.SetDuration(30f);
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        while (Time > 0f)
        {
            Timer.SetTime(Time);
            Time -= 1f;
            yield return new WaitForSeconds(1f);
        }

        yield return 0;
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
          
        }
    }
}
