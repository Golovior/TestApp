using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestApp
{
    internal class TestVragen
    {
        List<List<string>> appTestVragen;
        readonly string filePath;
        readonly string fileName;

        public TestVragen()
        {
            this.filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/widmTest";
            this.fileName = filePath + "/testvragen.txt";

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
                this.appTestVragen = new();
                return;
            }

            this.appTestVragen = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(json) ?? new();
        }

        public bool QuestionAlreadyExists(string test, string opdracht, string question)
        {
            return appTestVragen.Any(currentQuestionInTest =>
                    currentQuestionInTest.Count >= 3
                    && currentQuestionInTest[0] == test
                    && currentQuestionInTest[1] == opdracht
                    && currentQuestionInTest[2] == question);
        }

        public List<List<string>> GetAllTestVragen()
        {
            return this.appTestVragen;
        }

        public void AddTestVraag(string test, string opdracht, string question, string order)
        {
            List<string> currentTestVraag = new()
            {
                test,
                opdracht,
                question,
                order
            };

            appTestVragen.Add(currentTestVraag);

            this.SaveTestVragen();
        }

        public void RemoveTestVragen(List<string> vraag)
        {
            if(appTestVragen.Contains(vraag))
                appTestVragen.Remove(vraag);

            this.SaveTestVragen();
        }

        public string GetTestVragenInfo()
        {
            return JsonSerializer.Serialize(appTestVragen);
        }

        public void SaveTestVragen()
        {
            string json = JsonSerializer.Serialize(appTestVragen);
            File.WriteAllText(fileName, json);
        }

        public void UpdateFromApi(string data)
        {
            this.appTestVragen = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(data) ?? new();
            File.WriteAllText(fileName, data);
        }

    }
}

