using UnityEngine;

public static class UiUtility
{
    /// <summary>
    /// Adds a gameobject to a UI with the correct scale values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <param name="prototype"></param>
    /// <param name="enable">Specify if you want the child to be set active upon creation.</param>
    /// <returns></returns>
    public static T AddChild<T>(RectTransform parent, T prototype, bool enable = false) where T : MonoBehaviour
    {
        var instance = GameObject.Instantiate<T>(prototype);
        instance.transform.SetParent(parent.transform);
        instance.transform.localScale = Vector2.one;

        if (enable)
            instance.gameObject.SetActive(true);

        return instance;
    }
}
