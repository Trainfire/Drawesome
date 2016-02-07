using UnityEngine;
using System.Collections;

public class UiMenu : MonoBehaviour
{
    public virtual void Hide()
    {
        OnHide();
    }

    public virtual void Show()
    {
        OnShow();
    }

    protected virtual void OnHide()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnShow()
    {
        gameObject.SetActive(true);
    }
}
