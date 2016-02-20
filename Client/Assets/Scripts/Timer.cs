using UnityEngine;
using System.Collections;
using Protocol;

public class Timer : MonoBehaviour, Game.IGameStateHandler, Game.IGameMessageHandler
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
        Hide();
    }

    void Show()
    {
        View.gameObject.SetActive(true);
    }

    void Hide()
    {
        View.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    void SetDuration(float duration)
    {
        Duration = duration;
        CurrentTime = duration;
        View.Fill.fillAmount = 1f;
        shouldLerp = false;
    }

    void SetTime(float currentTime)
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

    void Game.IGameStateHandler.HandleState(GameState state)
    {
        
    }

    void Game.IGameMessageHandler.HandleMessage(string json)
    {
        // Change state
        Message.IsType<ServerMessage.Game.ChangeState>(json, (data) =>
        {
            Hide();
        });

        // Show and set timer
        Message.IsType<ServerMessage.Game.AddTimer>(json, (data) =>
        {
            Show();
            SetDuration(data.Duration);
        });

        // Update timer
        Message.IsType<ServerMessage.Game.SetTimer>(json, (data) =>
        {
            SetTime(data.CurrentTime);
        });
    }
}
