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
                settings = Load();
            return settings;
        }
        set
        {
            Settings = settings;
        }
    }

    static Settings Load()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(SettingsFileName, "")))
        {
            var file = PlayerPrefs.GetString(SettingsFileName);
            return JsonConvert.DeserializeObject<Settings>(file);
        }
        else
        {
            Console.WriteLine("Failed to find '{0}'. Making default...", SettingsFileName);
            return new Settings();
        }
    }

    static string GetFileName(string fileName)
    {
        return Prefix + "." + fileName;
    }
}
