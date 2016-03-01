using System;
using Newtonsoft.Json;
using System.IO;

namespace Server
{
    public class SettingsLoader : ILogger
    {
        public event EventHandler OnSettingsChanged;

        public Settings Values { get; private set; }

        const string Config = "config";
        const string Server = "server";
        const string Prompts = "prompts";
        const string Drawesome = "drawesome";
        const string Suffix = "settings";

        string RootDirectory { get; set; }

        string ILogger.LogName { get { return "Settings Loader"; } }

        public SettingsLoader()
        {
            RootDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public void Load()
        {
            var config = Load<ConfigSettings>(Config);
            var server = LoadFromUrl<ServerSettings>(config.ServerSettingsUrl, Server);
            var drawesome = LoadFromUrl<DrawesomeSettings>(config.DrawesomeSettingsUrl, Drawesome);
            var prompts = LoadFromUrl<PromptSettings>(config.PromptsUrl, Prompts);

            Logger.Log(this, "Decoys: {0}", drawesome.Decoys.Count);
            Logger.Log(this, "Prompts: {0}", prompts.Items.Count);
            Logger.Log(this, "Min Players: {0}", server.MaxPlayers);
            Logger.Log(this, "Max Players: {0}", server.MaxPlayers);
            Logger.Log(this, "Max Rooms: {0}", server.MaxRooms);

            Values = new Settings(config, server, drawesome, prompts);

            if (OnSettingsChanged != null)
                OnSettingsChanged(this, null);
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

        T LoadFromUrl<T>(string url, string defaultFileName)
        {
            using (WebClientEx wc = new WebClientEx())
            {
                try
                {
                    var json = wc.DownloadString(url);
                    Logger.Log("Loading '{0}' from URL '{1}'...", GetFileName(defaultFileName), url);
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch
                {
                    Logger.Warn(this, "Failed to load '{0}' from URL '{1}'", GetFileName(defaultFileName), url);
                    return MakeDefault<T>(defaultFileName);
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
            return fileName + "." + Suffix;
        }
    }
}
