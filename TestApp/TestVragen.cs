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
        readonly List<List<string>> appTestVragen;
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

            if (json == null)
            {
                this.appTestVragen = new();
                return;
            }

            this.appTestVragen = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(json) ?? new();
        }

        public bool QuestionAlreadyExists(string test, string opdracht, string question)
        {
            for (int i = 0; i < 50; i++)
            {
                List<string> currentQuestionInTest = new()
                {
                    test,
                    opdracht,
                    question,
                    Convert.ToString(i)
                };

                if (appTestVragen.Contains(currentQuestionInTest))
                    return true;
            }

            return false;
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
        }

        public string GetTestVragenInfo()
        {
            string allTestVragen = "[";

            foreach (List<string> testVragen in appTestVragen)
            {
                if (testVragen.Count < 3)
                    continue;

                if (allTestVragen.Length > 2)
                    allTestVragen += ",";

                allTestVragen += "['" + testVragen[0] + "','" + testVragen[1] + "','" + testVragen[2] + "','" + testVragen[3] + "']";
            }

            allTestVragen += "]";

            return allTestVragen;
        }

        public void SaveTestVragen()
        {
            string json = JsonSerializer.Serialize(appTestVragen);
            File.WriteAllText(fileName, json);
        }

    }
}
