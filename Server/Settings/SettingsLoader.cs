using System;
using Newtonsoft.Json;
using System.IO;

namespace Server
{
    public class SettingsLoader : ILogger
    {
        public event EventHandler OnSettingsChanged;

        public Settings Values { get; private set; }

        const string Server = "server";
        const string Prompts = "prompts";
        const string Drawesome = "drawesome";
        const string Prefix = "settings";

        string RootDirectory { get; set; }

        string ILogger.LogName { get { return "Settings Loader"; } }

        public SettingsLoader()
        {
            RootDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public void Load()
        {
            var server = Load<ServerSettings>(Server);
            var drawesome = LoadFromUrl<DrawesomeSettings>(server.DrawesomeSettingsUrl, Drawesome);
            var prompts = LoadFromUrl<PromptSettings>(server.PromptsUrl, Prompts);

            Logger.Log(this, "Decoys: {0}", drawesome.Decoys.Count);
            Logger.Log(this, "Prompts: {0}", prompts.Items.Count);
            Logger.Log(this, "Min Players: {0}", server.MaxPlayers);
            Logger.Log(this, "Max Players: {0}", server.MaxPlayers);
            Logger.Log(this, "Max Rooms: {0}", server.MaxRooms);

            Values = new Settings(server, drawesome, prompts);

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
}
