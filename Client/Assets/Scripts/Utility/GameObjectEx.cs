using UnityEngine;
using System.Collections.Generic;

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

    public static List<Transform> GetChildTransforms(this GameObject obj, bool includeInActive = false)
    {
        var children = obj.GetComponentsInChildren<Transform>(includeInActive);
        var childTransforms = new List<Transform>();
        foreach (var child in children)
        {
            if (child.parent == obj.transform)
            {
                childTransforms.Add(child);
            }
        }
        return childTransforms;
    }
}
