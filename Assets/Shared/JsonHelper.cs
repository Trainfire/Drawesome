namespace Protocol
{
    /// <summary>
    /// Allow JSON conversion between Server and Client through same method call.
    /// </summary>
    static class JsonHelper
    {
        public static T FromJson<T>(string json)
        {
#if UNITY_5
            return UnityEngine.JsonUtility.FromJson<T>(json);
#else
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
#endif
        }

        public static string ToJson(object obj)
        {
#if UNITY_5
            return UnityEngine.JsonUtility.ToJson(obj);
#else
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
#endif
        }
    }
}
