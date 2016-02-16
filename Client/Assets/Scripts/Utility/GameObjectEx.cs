using UnityEngine;

public static class GameObjectEx
{
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        var comp = obj.GetComponent<T>();
        if (comp != null)
        {
            return comp;
        }
        else
        {
            return obj.gameObject.AddComponent<T>();
        }
    }
}
