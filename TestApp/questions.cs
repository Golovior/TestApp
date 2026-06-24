using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Questions
    {
        List<List<string>> appQuestions;
        readonly string filePath;
        readonly string fileName;

        public Questions() {
            this.filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/widmTest";
            this.fileName = filePath + "/questions.txt";

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
                this.appQuestions = new();
                return;
            }

            this.appQuestions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(json) ?? new();
        }

        public bool QuestionAlreadyExists(string opdracht, string question) {
            foreach (List<string> q in appQuestions)
            {
                if (q[0] != opdracht)
                    continue;

                if (q[1].ToLower() != question.ToLower())
                    continue;

                return true;
            }

            return false;
        }

        public List<List<string>> GetAllQuestions()
        {
            appQuestions.Sort((a, b) => a[1].CompareTo(b[1]));
            return this.appQuestions;
        }

        public void AddQuestion(string opdracht, string question, string alphabetical) {
            List<string> currentQuestion = new()
            {
                opdracht,
                question,
                alphabetical
            };

            appQuestions.Add(currentQuestion);

            this.SaveQuestions();
        }

        public string GetQuestionInfo()
        {
            return JsonSerializer.Serialize(appQuestions);
        }

        public void SaveQuestions()
        {
            string json = JsonSerializer.Serialize(appQuestions);
            File.WriteAllText(fileName, json);
        }

        public void UpdateFromApi(string data)
        {
            this.appQuestions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(data) ?? new();
            File.WriteAllText(fileName, data);
        }

    }
}

