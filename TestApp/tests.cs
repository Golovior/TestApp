using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Tests
    {
        List<string> appTests;
        readonly string filePath;
        readonly string fileName;

        public Tests()
        {
            this.filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/widmTest";
            this.fileName = filePath + "/tests.txt";

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
                this.appTests = new();
                return;
            }

            this.appTests = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json) ?? new();
        }

        public bool TestAlreadyExists(string test)
        {
            foreach (string t in appTests)
            {
                if (t.ToLower() == test.ToLower())
                    return true;
            }

            return false;
        }

        public List<string> GetAllTests()
        {
            this.appTests.Sort();
            return this.appTests;
        }

        public void AddTest(string test)
        {
            appTests.Add(test);

            this.SaveTests();
        }

        public string GetTestInfo()
        {
            return JsonSerializer.Serialize(appTests);
        }

        public void SaveTests()
        {
            string json = JsonSerializer.Serialize(appTests);
            File.WriteAllText(fileName, json);
        }

        public void UpdateFromApi(string data)
        {
            this.appTests = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(data) ?? new();
            File.WriteAllText(fileName, data);
        }
    }
}
