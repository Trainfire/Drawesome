using UnityEngine;

public static class UiUtility
{
    public static T AddChild<T>(RectTransform parent, T prototype) where T : MonoBehaviour
    {
        var instance = GameObject.Instantiate<T>(prototype);
        instance.transform.localScale = Vector2.one;
        instance.transform.SetParent(parent.transform);
        return instance;
    }
}
