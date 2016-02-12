using UnityEngine;
using UnityEngine.UI;

public class UiTimer : MonoBehaviour
{
    public Image Fill;
    public Text Label;

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
