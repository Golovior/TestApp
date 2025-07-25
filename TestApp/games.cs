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
        readonly List<string> appGames;
        readonly string filePath;
        readonly string fileName;

        public Games() {
            this.filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/widmTest";
            this.fileName = filePath + "/games.txt";

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            if (!File.Exists(fileName))
            {
                var createdFile = File.Create(fileName);
                createdFile.Close();
            }

            string json = File.ReadAllText(fileName);

            if (json == null)
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
            string allGames = "[";

            foreach (string game in appGames) {
                if (allGames.Length > 2)
                    allGames += ",";

                allGames += "'" + game + "'";
            }

            allGames += "]";

            return allGames;
        }

        public void SaveGames()
        {
            string json = JsonSerializer.Serialize(appGames);
            File.WriteAllText(fileName, json);
        }

    }
}
