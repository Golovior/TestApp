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
        readonly List<string> appOpdrachten;
        readonly string filePath;
        readonly string fileName;

        public Opdrachten()
        {
            this.filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/widmTest";
            this.fileName = filePath + "/opdrachten.txt";

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
                this.appOpdrachten = new();
                return;
            }

            this.appOpdrachten = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json) ?? new();
        }

        public List<string> GetOpdrachten()
        {
            return this.appOpdrachten;
        }

        public bool OpdrachtAlreadyExists(string name)
        {
            if (appOpdrachten.Contains(name))
                return true;

            return false;
        }

        public void AddOpdracht(string name)
        {
            appOpdrachten.Add(name);

            this.SaveOpdrachten();
        }

        public string GetOpdrachtenInfo()
        {
            string allOpdachten = "[";

            foreach (string opdracht in appOpdrachten)
            {
                if (allOpdachten.Length > 2)
                    allOpdachten += ",";

                allOpdachten += "'" + opdracht + "'";
            }

            allOpdachten += "]";

            return allOpdachten;
        }

        public void SaveOpdrachten()
        {
            string json = JsonSerializer.Serialize(appOpdrachten);
            File.WriteAllText(fileName, json);
        }

    }
}
