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
        readonly List<List<string>> appTestAntwoorden;
        readonly string filePath;
        readonly string fileName;

        public TestAntwoorden()
        {
            this.filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/widmTest";
            this.fileName = filePath + "/testantwoorden.txt";

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
            string allTestAntwoorden = "[";

            foreach (List<string> testAntwoorden in appTestAntwoorden)
            {
                if (testAntwoorden.Count < 3)
                    continue;

                if (allTestAntwoorden.Length > 2)
                    allTestAntwoorden += ",";

                allTestAntwoorden += "['" + testAntwoorden[0] + "','" + testAntwoorden[1] + "','" + testAntwoorden[2] + "','" + testAntwoorden[3] + "','" + testAntwoorden[4] + "']";
            }

            allTestAntwoorden += "]";

            return allTestAntwoorden;
        }

        public void SaveTestAntwoorden()
        {
            string json = JsonSerializer.Serialize(appTestAntwoorden);
            File.WriteAllText(fileName, json);
        }

    }
}
