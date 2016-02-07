using UnityEngine;
using System.Collections;

public class UiMenu : MonoBehaviour
{
    public virtual void OnHide()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnShow()
    {
        gameObject.SetActive(true);
    }
}
