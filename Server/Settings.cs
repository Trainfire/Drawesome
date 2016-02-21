using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Protocol;

namespace Server
{
    public class SettingsLoader
    {
        const string Server = "server";
        const string Drawesome = "drawesome";
        const string Prompts = "prompts";

        const string Prefix = "settings";
        string RootDirectory { get; set; }

        public SettingsLoader()
        {
            RootDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public Settings Load()
        {
            var server = Load<ServerSettings>(Server);
            var drawesome = Load<DrawesomeSettings>(Drawesome);
            var prompts = Load<PromptSettings>(Prompts);
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
        public int MaxPlayers = 1;
        public int NameMinChars = 3;
        public int NameMaxChars = 24;
    }

    public class DrawesomeSettings
    {
        public float RoundBeginTime = 5f;
        public float DrawTime = 60f;
        public float AnsweringTime = 30f;
        public float ChoosingTime = 30f;
        public float ResultTimeBetween = 10f;
        public uint PointsForFakeAnswer = 100;
        public uint PointsForCorrectAnswer = 200;
        public uint PointsToDrawerForCorrectAnswer = 1000;

        public List<string> Decoys = new List<string>();

        public TransitionData Transitions = new TransitionData();

        public class TransitionData
        {
            public float RoundBeginToDrawing;
            public float DrawingToAnswering;
            public float AnsweringToChoosing;
            public float ChoosingtoResults;
            public float ResultsToScores;
            public float ScoresToAnswering;
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
