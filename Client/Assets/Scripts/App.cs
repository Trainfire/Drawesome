using UnityEngine;
using System.Collections.Generic;

public class App : Singleton<App>
{
    List<Component> components = new List<Component>();

    protected override void Awake()
    {
        base.Awake();
    }

    public T GetInstance<T>() where T : Component
    {
        foreach (var c in components)
        {
            if (c.GetType() == typeof(T))
                return (T)c;
        }

        var find = FindObjectOfType<T>();
        if (find)
        {
            components.Add(find.GetComponent<T>());
            DontDestroyOnLoad(find.gameObject);
            return find;
        }
           
        return null;
    }

    void MakeInstance<T>(GameObject prototype) where T : Component
    {
        var instance = Instantiate(prototype);
        DontDestroyOnLoad(instance);
        components.Add(instance.GetComponent<T>());
    }
}
