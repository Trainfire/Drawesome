using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class ConsoleView : MonoBehaviour
{
    public InputField InputField;
    public RectTransform LogView;
    public Text LogText;

    ConsoleController Console { get; set; }

    const float LogHeight = 250f;
    const float LogToggleTime = 0.2f;
    AnimationCurve LogAnimCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    enum State
    {
        Closed,
        Open,
    }
    State ConsoleState;

    void Awake()
    {
        Console = new ConsoleController();

        LogText.text = "";
        InputField.text = "";
        LogView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);

        // Add callback for all Unity Debug Log events.
        Application.RegisterLogCallbackThreaded(OnLogMessageRecieved);
    }

    void OnLogMessageRecieved(string condition, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                LogText.text += "<color=#f00>Error: " + condition + "</color>\r\n";
                break;
            case LogType.Assert:
                LogText.text += "<color=#f00>Assert: " + condition + "</color>\r\n";
                break;
            case LogType.Warning:
                break;
            case LogType.Log:
                LogText.text += condition + "\r\n";
                break;
            case LogType.Exception:
                break;
            default:
                break;
        }
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            Console.SubmitInput(InputField.text);
            InputField.text = "";
            Focus();
        }

        if (Input.GetKeyUp(KeyCode.Tab) || Input.GetKeyUp(KeyCode.F1))
        {
            StopAllCoroutines();
            StartCoroutine(Toggle());
            Focus();
        }
    }

    void Focus()
    {
        // Hack to keep focus on inputfield after submitting a command. Classic Unity.
        EventSystem.current.SetSelectedGameObject(InputField.gameObject, null);
        InputField.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    IEnumerator Toggle()
    {
        Debug.Log("Toggle");

        float time = 0f;

        if (ConsoleState == State.Closed)
        {
            ConsoleState = State.Open;

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

            // Set to closed
            LogView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
        }
    }
}
