using UnityEngine;
using System;

public class AFKChecker : MonoBehaviour
{
    public event Action OnStatusChanged;
    public bool AFK { get; set; }

    float timeTillAfk = 300f;
    public float TimeTillAfk { get { return timeTillAfk; } set { timeTillAfk = value; } }

    Vector3 lastMousePosition = Vector3.one;
    float timeSinceLastActivity = 0f;
    bool previousAfkStatus = false;

    void LateUpdate()
    {
        if (Input.anyKeyDown)
            MakeTimeStamp();

        if (!Mathf.Approximately(lastMousePosition.x, Input.mousePosition.x))
        {
            lastMousePosition = Input.mousePosition;
            MakeTimeStamp();
        }
    }

    void MakeTimeStamp()
    {
        timeSinceLastActivity = Time.time;
    }

    void Update()
    {
        AFK = Time.time > timeSinceLastActivity + timeTillAfk;

        if (previousAfkStatus != AFK)
        {
            OnStatusChanged();
            previousAfkStatus = AFK;
        }
    }
}
