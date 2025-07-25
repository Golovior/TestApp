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
        readonly List<List<string>> appQuestions;
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

            if (json == null)
            {
                this.appQuestions = new();
                return;
            }

            this.appQuestions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(json) ?? new();
        }

        public bool QuestionAlreadyExists(string opdracht, string question, string alphabetical) {
            List<string> currentQuestion = new()
            {
                opdracht,
                question,
                alphabetical
            };

            if (appQuestions.Contains(currentQuestion))
                return true;

            return false;
        }

        public List<List<string>> GetAllQuestions()
        {
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
            string allQuestions = "[";

            foreach (List<string> questions in appQuestions) {
                if(questions.Count < 2)
                    continue;

                if (allQuestions.Length > 2)
                    allQuestions += ",";

                allQuestions += "['" + questions[0] + "','" + questions[1] + "','" + questions[2] + "']";
            }

            allQuestions += "]";

            return allQuestions;
        }

        public void SaveQuestions()
        {
            string json = JsonSerializer.Serialize(appQuestions);
            File.WriteAllText(fileName, json);
        }

    }
}
