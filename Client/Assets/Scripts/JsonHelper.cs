using UnityEngine;

static class JsonHelper
{
    public static T FromJson<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }

    public static string ToJson(object obj)
    {
        var json = JsonUtility.ToJson(obj);
        Debug.Log("Json: " + json);
        return "";
    }
}
