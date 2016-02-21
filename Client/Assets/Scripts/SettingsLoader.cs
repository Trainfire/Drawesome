using UnityEngine;
using System;
using Newtonsoft.Json;

public static class SettingsLoader
{
    const string SettingsFileName = "settings";
    const string Prefix = "";

    static Settings settings;
    public static Settings Settings
    {
        get
        {
            if (settings == null)
                settings = Load<Settings>(SettingsFileName);
            return settings;
        }
        set
        {
            Settings = settings;
        }
    }

    static Settings Load()
    {
        return Load<Settings>(SettingsFileName);
    }

    static T Load<T>(string prefKey)
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(SettingsFileName, "")))
        {
            var file = PlayerPrefs.GetString(prefKey);
            return JsonConvert.DeserializeObject<T>(file);
        }
        else
        {
            Console.WriteLine("Failed to find '{0}'. Making default...", prefKey);
            return MakeDefault<T>();
        }
    }

    static T MakeDefault<T>()
    {
        T data = Activator.CreateInstance<T>();
        return data;
    }

    static string GetFileName(string fileName)
    {
        return Prefix + "." + fileName;
    }
}
