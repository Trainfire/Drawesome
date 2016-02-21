using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

public static class SettingsLoader
{
    const string SettingsFileName = "main";

    const string FileExtension = ".settings";
    static string RootDirectory
    {
        get
        {
            return Application.dataPath;
        }
    }

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

    static T Load<T>(string fileName)
    {
        var path = Path.Combine(RootDirectory, GetFileName(fileName));

        if (File.Exists(path))
        {
            var file = File.ReadAllText(path);
            Debug.Log("Found settings");
            return JsonConvert.DeserializeObject<T>(file);
        }
        else
        {
            Debug.LogWarningFormat("Failed to find '{0}'. Making default...", fileName);
            return MakeDefault<T>(fileName);
        }
    }

    static void Save<T>(T data, string fileName)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(RootDirectory + GetFileName(fileName), json);
    }

    static T MakeDefault<T>(string fileName)
    {
        T data = Activator.CreateInstance<T>();
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(GetFileName(fileName), json);
        return data;
    }

    static string GetFileName(string fileName)
    {
        return SettingsFileName + FileExtension;
    }
}
