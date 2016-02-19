using UnityEngine;
using System.Collections;

public class TestPopup : MonoBehaviour
{
    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            PopupFactory.Instance.MakePopup("Hi!", () => Debug.Log("Ayyyy")).Show();
        }
    }
}
