using UnityEngine;
using UnityEngine.Events;

public abstract class UiTween<T> : MonoBehaviour
{
    public UnityAction<T> OnTweenValue;
    public float Duration;

    public bool DoTween = false;

    bool Tweening { get; set; }
    float CurrentTime { get; set; }
    UnityAction OnDone { get; set; }

    public void Play(UnityAction onDone = null)
    {
        CurrentTime = 0f;
        DoTween = true;
        OnDone = onDone;
    }

    void Update()
    {
        if (DoTween)
        {
            DoTween = false;
            Tweening = true;
            CurrentTime = 0f;
        }

        if (Tweening)
        {
            T value = OnTween(CurrentTime / Duration);

            if (OnTweenValue != null)
                OnTweenValue(value);

            CurrentTime += Time.deltaTime;

            if (CurrentTime > Duration)
            {
                Tweening = false;
                if (OnDone != null)
                    OnDone();
            }
        }
    }

    protected abstract T OnTween(float delta);
}
