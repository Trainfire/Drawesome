using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ConsoleView : MonoBehaviour
{
    public GameObject View;
    public InputField InputField;
    public RectTransform LogView;
    public ConsoleLogView LogViewItem;
    public RectTransform LogItemContainer;
    public ScrollRect LogScrollView;

    ConsoleController Console { get; set; }

    const float LogHeight = 250f;
    const float LogToggleTime = 0.2f;
    const float LogTimeToLive = 60f;
    AnimationCurve LogAnimCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    enum State
    {
        Closed,
        Open,
    }
    State ConsoleState;

    public void SetConsole(ConsoleController console)
    {
        Console = console;

        LogViewItem.gameObject.SetActive(false);
        InputField.text = "";
        LogView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);

        // Add callback for all Unity Debug Log events.
        Application.logMessageReceivedThreaded += OnLogMessageRecieved;

        View.SetActive(false);
    }

    void OnLogMessageRecieved(string condition, string stackTrace, LogType type)
    {
        var logView = Instantiate(LogViewItem);
        logView.transform.SetParent(LogItemContainer);

        string log = "";
        switch (type)
        {
            case LogType.Error:
                log += "<color=#f00>Error: " + condition + "</color>";
                break;
            case LogType.Assert:
                log += "<color=#f00>Assert: " + condition + "</color>";
                break;
            case LogType.Warning:
                break;
            case LogType.Log:
                log += condition;
                break;
            case LogType.Exception:
                break;
            default:
                break;
        }

        logView.SetText(log, LogTimeToLive);
    }

    void Update()
    {
        // Scroll to bottom
        LogScrollView.ScrollToBottom();
    }

    void LateUpdate()
    {
        if (ConsoleState == State.Open && Input.GetKeyUp(KeyCode.Return))
        {
            Console.SubmitInput(InputField.text);
            InputField.text = "";
            InputField.Focus();
        }

        if (Input.GetKeyUp(KeyCode.Tab) || Input.GetKeyUp(KeyCode.F1))
        {
            StopAllCoroutines();
            StartCoroutine(Toggle());
            InputField.Focus();
        }
    }

    IEnumerator Toggle()
    {
        float time = 0f;

        if (ConsoleState == State.Closed)
        {
            ConsoleState = State.Open;

            View.SetActive(true);

            while (time < LogToggleTime)
            {
                LogView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(0f, LogHeight, LogAnimCurve.Evaluate(time / LogToggleTime)));
                time += Time.deltaTime;
                yield return 0;
            }

            // Set to open
            LogView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, LogHeight);
        }
        else
        {
            ConsoleState = State.Closed;

            while (time < LogToggleTime)
            {
                LogView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(LogHeight, 0f, LogAnimCurve.Evaluate(time / LogToggleTime)));
                time += Time.deltaTime;
                yield return 0;
            }

            View.SetActive(false);

            // Set to closed
            LogView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
        }
    }
}
