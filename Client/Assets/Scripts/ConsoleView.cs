using UnityEngine;
using UnityEngine.UI;

public class ConsoleView : MonoBehaviour
{
    public InputField InputField;
    public Text Log;

    ConsoleController Console { get; set; }

    void Awake()
    {
        Console = new ConsoleController();

        Log.text = "";
        InputField.text = "";

        Console.Log += (str) =>
        {
            Log.text += "\r\n " + str;
        };
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            Console.SubmitInput(InputField.text);
            InputField.text = "";
        }
    }
}
