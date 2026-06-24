using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Games
    {
        List<string> appGames;
        readonly string filePath;
        readonly string fileName;

        public Games() {
            this.filePath = AppStoragePaths.DataDirectory;
            this.fileName = Path.Combine(filePath, "games.txt");

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            if (!File.Exists(fileName))
            {
                var createdFile = File.Create(fileName);
                createdFile.Close();
            }

            string json = File.ReadAllText(fileName);

            if (string.IsNullOrWhiteSpace(json))
            {
                this.appGames = new();
                return;
            }

            this.appGames = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json) ?? new();
        }

        public bool GameAlreadyExists(string name) {
            if (appGames.Contains(name))
                return true;

            return false;
        }

        public void AddGame(string name) {
            appGames.Add(name);

            this.SaveGames();
        }

        public string GetGameInfo()
        {
            return JsonSerializer.Serialize(appGames);
        }

        public void SaveGames()
        {
            string json = JsonSerializer.Serialize(appGames);
            File.WriteAllText(fileName, json);
        }

        public void UpdateFromApi(string data)
        {
            this.appGames = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(data) ?? new();
            File.WriteAllText(fileName, data);
        }

    }
}

