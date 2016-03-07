using System;
using System.Collections.Generic;
using Protocol;

namespace Server
{
    public class Settings
    {
        public ConfigSettings Config { get; private set; }
        public ServerSettings Server { get; private set; }
        public DrawesomeSettings Drawesome { get; private set; }
        public PromptSettings Prompts { get; private set; }

        public Settings()
        {
            Config = new ConfigSettings();
            Server = new ServerSettings();
            Drawesome = new DrawesomeSettings();
            Prompts = new PromptSettings();
        }

        public Settings(ConfigSettings config, ServerSettings server, DrawesomeSettings drawesome, PromptSettings prompts)
        {
            Config = config;
            Server = server;
            Drawesome = drawesome;
            Prompts = prompts;
        }
    }

    /// <summary>
    /// Settings file that must be hosted alongside the server.
    /// </summary>
    public class ConfigSettings
    {
        public string AdminPassword = "1111";
        public string ServerSettingsUrl = "http://drawesome.trainfire.net/server.settings";
        public string ClientSettingsUrl = "http://drawesome.trainfire.net/client.settings";
        public string PromptsUrl = "http://drawesome.trainfire.net/prompts.settings";
        public string DrawesomeSettingsUrl = "http://drawesome.trainfire.net/drawesome.settings";
    }

    public class ServerSettings : ServerData
    {
        public int MinPlayers = 3;
        public int MaxPlayers = 8;
        public int MaxRooms = 1;
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
