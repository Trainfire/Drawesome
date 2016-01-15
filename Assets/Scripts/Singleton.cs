using UnityEngine;
using System.Collections;
using System.Linq;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;
    public static T Instance
    {
        get
        {
            var findInstances = FindObjectsOfType<T>();
            if (findInstances.Length != 0)
                instance = findInstances[0];

            if (instance == null)
            {
                var newInstance = new GameObject();
                instance = newInstance.AddComponent<T>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }
}
