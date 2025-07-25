using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Settings
    {
        readonly List<string> keys = new();
        readonly List<KeyValuePair<string, string>> pairs;
        readonly string filePath;
        readonly string fileName;

        public Settings() {
            this.filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/widmTest";
            this.fileName = filePath + "/settings.txt";

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
                this.pairs = new();
                return;
            }

            this.pairs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(json) ?? new();

            keys.Add("ActiveGame");
        }

        public List<string> GetKeys()
        {
            return keys;
        }

        public void UpdateSetting(string key, string value) {
            if (keys.Contains(key))
            {
                KeyValuePair<string, string> pair = new(key, value);

                foreach (KeyValuePair<string, string> kv in pairs)
                {
                    if (kv.Key == key)
                        pairs.Remove(kv);
                }

                pairs.Add(pair);
            }
        }

        public string GetSettingsInfo()
        {
            string allSettings = "[";

            foreach (KeyValuePair<string, string> pair in pairs)
            {
                if (allSettings.Length > 2)
                    allSettings += ",";

                allSettings += "{'" + pair.Key + "':'" + pair.Value + "'}";
            }

            allSettings += "]";

            return allSettings;
        }

    }
}
