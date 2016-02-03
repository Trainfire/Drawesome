using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

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

        Console.Log += (str) =>
        {
            LogText.text += "\r\n " + str;
        };
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            Console.SubmitInput(InputField.text);
            InputField.text = "";

            // Hack to keep focus on inputfield after submitting a command. Classic Unity.
            EventSystem.current.SetSelectedGameObject(InputField.gameObject, null);
            InputField.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        if (Input.GetKeyUp(KeyCode.Tab) || Input.GetKeyUp(KeyCode.F1))
        {
            StopAllCoroutines();
            StartCoroutine(Toggle());
        }
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