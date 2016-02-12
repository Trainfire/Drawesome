using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server
{
    public class SettingsLoader
    {
        const string FilePath = "settings";

        public Settings Load()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FilePath);

            if (File.Exists(path))
            {
                var file = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<Settings>(file);
            }
            else
            {
                Console.WriteLine("Failed to find ServerSettings. Making default...");
                return MakeDefault();
            }
        }

        public Settings MakeDefault()
        {
            var settings = new Settings();
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(FilePath, json);
            return settings;
        }
    }

    public class Settings
    {
        public Server ServerSettings;
        public Drawesome DrawesomeSettings;

        public Settings()
        {
            ServerSettings = new Settings.Server();
            DrawesomeSettings = new Settings.Drawesome();
        }

        public class Server
        {
            public string HostUrl = "ws://0.0.0.0:8080";
        }

        public class Drawesome
        {
            public float RoundBeginTime = 5f;
            public float DrawTime = 60f;
            public float AnsweringTime = 30f;
            public float ChoosingTime = 30f;
            public List<string> Prompts = new List<string>()
            {
                "Prompt 1",
                "Prompt 2",
            };
        }
    }
}
