using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Protocol;
using System.Net;

namespace Server
{
    public class SettingsLoader
    {
        const string Server = "server";
        const string Prompts = "prompts";
        const string Drawesome = "drawesome";
        const string Prefix = "settings";
        string RootDirectory { get; set; }

        public SettingsLoader()
        {
            RootDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public Settings Load()
        {
            var server = Load<ServerSettings>(Server);
            var drawesome = LoadFromUrl<DrawesomeSettings>(server.DrawesomeSettingsUrl, Drawesome);
            var prompts = LoadFromUrl<PromptSettings>(server.PromptsUrl, Prompts);

            Console.WriteLine("Decoys: {0}", drawesome.Decoys.Count);
            Console.WriteLine("Prompts: {0}", prompts.Items.Count);

            return new Settings(server, drawesome, prompts);
        }

        T Load<T>(string fileName)
        {
            var path = Path.Combine(RootDirectory, GetFileName(fileName));

            if (File.Exists(path))
            {
                var file = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(file);
            }
            else
            {
                Console.WriteLine("Failed to find '{0}'. Making default...", fileName);
                return MakeDefault<T>(fileName);
            }
        }

        T LoadFromUrl<T>(string url, string fileName)
        {
            using (WebClientEx wc = new WebClientEx())
            {
                try
                {
                    var json = wc.DownloadString(url);
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch
                {
                    return MakeDefault<T>(fileName);
                }
            }
        }

        void Save<T>(T data, string fileName)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(RootDirectory + GetFileName(fileName), json);
        }

        T MakeDefault<T>(string fileName)
        {
            T data = Activator.CreateInstance<T>();
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(GetFileName(fileName), json);
            return data;
        }

        string GetFileName(string fileName)
        {
            return Prefix + "." + fileName;
        }
    }

    public class Settings
    {
        public ServerSettings Server { get; private set; }
        public DrawesomeSettings Drawesome { get; private set; }
        public PromptSettings Prompts { get; private set; }

        public Settings()
        {
            Server = new ServerSettings();
            Drawesome = new DrawesomeSettings();
            Prompts = new PromptSettings();
        }

        public Settings(ServerSettings server, DrawesomeSettings drawesome, PromptSettings prompts)
        {
            Server = server;
            Drawesome = drawesome;
            Prompts = prompts;
        }
    }

    public class ServerSettings
    {
        public string HostUrl = "ws://0.0.0.0:8080";
        public string PromptsUrl = "";
        public string DrawesomeSettingsUrl = "";
        public string AdminPassword = "doubleButts.exe"; // TODO: Generate this everytime the server runs?
        public int MaxPlayers = 1;
        public int NameMinChars = 3;
        public int NameMaxChars = 24;
    }

    public class DrawesomeSettings
    {
        public float RoundBeginTime = 0f;
        public float DrawTime = 60f;
        public float AnsweringTime = 30f;
        public float ChoosingTime = 30f;
        public float ResultTimeBetween = 15f;
        public float ScoresHoldTime = 12f;

        public uint PointsForFakeAnswer = 500;
        public uint PointsForCorrectAnswer = 1000;
        public uint PointsToDrawerForCorrectAnswer = 1000;

        public float TimeToShowResult = 8f;
        public float TimeToShowFinalResult = 10f;

        public List<string> Decoys = new List<string>()
        {
            "hammer & sickle",
            "i give up",
            "you are here",
            "what",
            "memes",
        };

        public TransitionData Transitions = new TransitionData();

        public class TransitionData
        {
            public float RoundBeginToDrawing = 1f;
            public float DrawingToAnswering = 4f;
            public float AnsweringToChoosing = 4f;
            public float ChoosingtoResults = 0f;
            public float ResultsToScores = 0f;
            public float ScoresToAnswering = 1f;
        }
    }

    public class PromptSettings
    {
        public List<PromptData> Items { get; set; }

        public PromptSettings()
        {
            Items = new List<PromptData>();
        }
    }
}
