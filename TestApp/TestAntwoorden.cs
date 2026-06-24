using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestApp
{
    internal class TestAntwoorden
    {
        List<List<string>> appTestAntwoorden;
        readonly string filePath;
        readonly string fileName;

        public TestAntwoorden()
        {
            this.filePath = AppStoragePaths.DataDirectory;
            this.fileName = Path.Combine(filePath, "testantwoorden.txt");

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
                this.appTestAntwoorden = new();
                return;
            }

            this.appTestAntwoorden = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(json) ?? new();
        }

        public bool AntwoordAlreadyExists(string test, string speler, string opdracht, string question, string antwoord)
        {
            List<string> currentGivenAnswerInTest = new()
            {
                test,
                speler,
                opdracht,
                question,
                antwoord
            };

            if (appTestAntwoorden.Contains(currentGivenAnswerInTest))
                return true;
            
            return false;
        }

        public List<List<string>> GetAllTestAntwoorden()
        {
            return this.appTestAntwoorden;
        }

        public void AddTestAntwoord(string test, string speler, string opdracht, string question, string antwoord)
        {
            List<string> currentTestAntwoord = new()
            {
                test,
                speler,
                opdracht,
                question,
                antwoord
            };

            appTestAntwoorden.Add(currentTestAntwoord);

            this.SaveTestAntwoorden();
        }

        public void RemoveTestAntwoord(List<string> antwoord)
        {
            if (appTestAntwoorden.Contains(antwoord))
                appTestAntwoorden.Remove(antwoord);
        }

        public string GetTestAntwoordenInfo()
        {
            return JsonSerializer.Serialize(appTestAntwoorden);
        }

        public void SaveTestAntwoorden()
        {
            string json = JsonSerializer.Serialize(appTestAntwoorden);
            File.WriteAllText(fileName, json);
        }

        public void UpdateFromApi(string data)
        {
            this.appTestAntwoorden = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(data) ?? new();
            File.WriteAllText(fileName, data);
        }

    }
}

