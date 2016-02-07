using UnityEngine;

public static class GameObjectEx
{
    public static T GetOrAddComponent<T>(this Component obj) where T : MonoBehaviour
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