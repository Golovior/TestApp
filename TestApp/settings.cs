using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Settings
    {
        readonly List<string> keys = new();
        List<KeyValuePair<string, string>> pairs;
        readonly string filePath;
        readonly string fileName;

        public Settings() {
            this.filePath = AppStoragePaths.DataDirectory;
            this.fileName = Path.Combine(filePath, "settings.txt");

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
                this.pairs = new();
            }
            else
            {
                this.pairs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(json) ?? new();
            }

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
            return JsonSerializer.Serialize(pairs);
        }

        public void UpdateFromApi(string data)
        {
            this.pairs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KeyValuePair<string,string>>>(data) ?? new();
            File.WriteAllText(fileName, data);
        }

    }
}
