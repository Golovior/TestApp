using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Opdrachten
    {
        List<string> appOpdrachten;
        readonly string filePath;
        readonly string fileName;

        public Opdrachten()
        {
            this.filePath = AppStoragePaths.DataDirectory;
            this.fileName = Path.Combine(filePath, "opdrachten.txt");

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
                this.appOpdrachten = new();
                return;
            }

            this.appOpdrachten = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json) ?? new();
        }

        public List<string> GetOpdrachten()
        {
            this.appOpdrachten.Sort();
            return this.appOpdrachten;
        }

        public bool OpdrachtAlreadyExists(string name)
        {
            foreach (string o in appOpdrachten)
            {
                if (o.ToLower() == name.ToLower())
                    return true;
            }

            return false;
        }

        public void AddOpdracht(string name)
        {
            appOpdrachten.Add(name);

            this.SaveOpdrachten();
        }

        public string GetOpdrachtenInfo()
        {
            return JsonSerializer.Serialize(appOpdrachten);
        }

        public void SaveOpdrachten()
        {
            string json = JsonSerializer.Serialize(appOpdrachten);
            File.WriteAllText(fileName, json);
        }

        public void UpdateFromApi(string data)
        {
            this.appOpdrachten = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(data) ?? new();
            File.WriteAllText(fileName, data);
        }

    }
}

