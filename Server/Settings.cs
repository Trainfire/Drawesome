using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server
{
    public static class SettingsHelper
    {
        public static Settings Settings { get; private set; }

        public static void Load(string url)
        {
            // Load settings here
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(url);
                Settings = JsonConvert.DeserializeObject<Settings>(json);
            }
        }

        public static void SaveToProjectFolder(Settings settings, string fileName)
        {
            string destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(destPath, json);
        }
    }

    /// <summary>
    /// Server Settings Here (Model)
    /// </summary>
    public class Settings
    {
        public class Server
        {

        }

        public class Game
        {
            public class Drawesome
            {
                public float RoundBeginTime { get; private set; }
                public float DrawTime { get; private set; }
                public float AnsweringTime { get; private set; }
                public float ChoosingTime { get; private set; }
                public List<string> Prompts { get; private set; }

                public Drawesome()
                {

                }
            }
        }
    }
}
