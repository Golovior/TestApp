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
        readonly List<string> appTests;
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

            if (json == null)
            {
                this.appTests = new();
                return;
            }

            this.appTests = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json) ?? new();
        }

        public bool TestAlreadyExists(string test)
        {
            if (appTests.Contains(test))
                return true;

            return false;
        }

        public List<string> GetAllTests()
        {
            return this.appTests;
        }

        public void AddTest(string test)
        {

            appTests.Add(test);

            this.SaveTests();
        }

        public string GetTestInfo()
        {
            string allTests = "[";

            foreach (string test in appTests)
            {
                if (allTests.Length > 2)
                    allTests += ",";

                allTests += "'" + test + "'";
            }

            allTests += "]";

            return allTests;
        }

        public void SaveTests()
        {
            string json = JsonSerializer.Serialize(appTests);
            File.WriteAllText(fileName, json);
        }
    }
}
