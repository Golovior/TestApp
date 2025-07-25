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
        readonly List<List<string>> appSpelers;
        readonly string filePath;
        readonly string fileName;

        public Spelers()
        {
            this.filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/widmTest";
            this.fileName = filePath + "/spelers.txt";

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
                this.appSpelers = new();
                return;
            }

            this.appSpelers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(json) ?? new();
        }

        public List<List<string>> GetSpelers()
        {
            return this.appSpelers;
        }

        public bool SpelerAlreadyExists(string name)
        {
            foreach(List<string> player in appSpelers)
            {
                if (player[0] == name)
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
            string allSpelers  = "[";

            foreach (List<string> speler in appSpelers)
            {
                if (allSpelers.Length > 2)
                    allSpelers += ",";

                allSpelers += "['" + speler[0] + "','" + speler[1] + "']";
            }

            allSpelers += "]";

            return allSpelers;
        }

        public void SaveSpelers()
        {
            string json = JsonSerializer.Serialize(appSpelers);
            File.WriteAllText(fileName, json);
        }

    }
}
