using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Spelers
    {
        List<List<string>> appSpelers;
        readonly string filePath;
        readonly string fileName;

        public Spelers()
        {
            this.filePath = AppStoragePaths.DataDirectory;
            this.fileName = Path.Combine(filePath, "spelers.txt");

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
                this.appSpelers = new();
                return;
            }

            this.appSpelers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(json) ?? new();
        }

        public List<List<string>> GetSpelers()
        {
            appSpelers.Sort((a, b) => a[0].CompareTo(b[0]));
            return this.appSpelers;
        }

        public bool SpelerAlreadyExists(string name)
        {
            foreach(List<string> player in appSpelers)
            {
                if (player[0].ToLower() == name.ToLower())
                    return true;
            }

            return false;
        }

        public void AddSpeler(string name)
        {
            List<string> player = new()
            {
                name,
                "1"
            };

            appSpelers.Add(player);

            this.SaveSpelers();
        }

        public void SavePlayerStatus(string name, string status)
        {
            foreach (List<string> speler in appSpelers)
            {
                if (speler[0] == name)
                    speler[1] = status;
            }

            this.SaveSpelers();
        }

        public string GetSpelersInfo()
        {
            return JsonSerializer.Serialize(appSpelers);
        }

        public void SaveSpelers()
        {
            string json = JsonSerializer.Serialize(appSpelers);
            File.WriteAllText(fileName, json);
        }

        public void UpdateFromApi(string data)
        {
            this.appSpelers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(data) ?? new();
            File.WriteAllText(fileName, data);
        }

    }
}

