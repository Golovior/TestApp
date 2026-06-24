using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestApp
{
    internal class Antwoorden
    {
        List<List<string>> appAntwoorden;
        readonly string filePath;
        readonly string fileName;

        public Antwoorden()
        {
            this.filePath = AppStoragePaths.DataDirectory;
            this.fileName = Path.Combine(filePath, "antwoorden.txt");

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
                this.appAntwoorden = new();
                return;
            }

            this.appAntwoorden = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(json) ?? new();
        }

        public List<List<string>> GetAntwoorden()
        {
            return this.appAntwoorden;
        }

        public bool AntwoordAlreadyExists(string opdracht, string vraag, string name)
        {
            foreach (List<string> antwoord in this.appAntwoorden)
            {
                if (opdracht != antwoord[0])
                    continue;

                if (vraag != antwoord[1])
                    continue;

                if (name.ToLower() != antwoord[2].ToLower())
                    continue;

                return true;
            }

            return false;
        }

        public void AddAntwoord(string opdracht, string vraag, string name, string correct = "0")
        {
            List<string> antwoord = new()
            {
                opdracht,
                vraag,
                name,
                correct,
                "[]"
            };

            appAntwoorden.Add(antwoord);

            this.SaveAntwoorden();
        }

        public string GetAntwoordenInfo()
        {
            return JsonSerializer.Serialize(appAntwoorden);
        }

        public void SetAsCorrectAntwoord(string opdracht, string vraag, string name)
        {
            foreach (List<string> antwoordSet in appAntwoorden)
            {
                if (antwoordSet[0] == opdracht && antwoordSet[1] == vraag)
                {
                    if (name == antwoordSet[2])
                        antwoordSet[3] = "1";
                    else
                        antwoordSet[3] = "0";
                }
            }

            SaveAntwoorden();
        }

        public void ConnectPlayersToAnswer(string opdracht, string vraag, string antwoord, List<string> spelers)
        {
            string connectedPlayers = JsonSerializer.Serialize(spelers);

            foreach (List<string> antwoordSet in appAntwoorden)
            {
                if (antwoordSet[0] == opdracht && antwoordSet[1] == vraag && antwoordSet[2] == antwoord)
                {
                    antwoordSet[4] = connectedPlayers;
                }
            }

            SaveAntwoorden();
        }

        public void SaveAntwoorden()
        {
            string json = JsonSerializer.Serialize(appAntwoorden);
            File.WriteAllText(fileName, json);
        }

        public void UpdateFromApi(string data)
        {
            appAntwoorden = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(data) ?? new();
            File.WriteAllText(fileName, data);
        }
    }
}

