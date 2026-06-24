using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Api
    {
        readonly string baseUrl;
        readonly DataSetClass ds;

        public Api(DataSetClass ds) {
            this.baseUrl = "http://widmtimer.fvandenberg.nl/api/";

            this.ds = ds;
        }

        public async Task SaveData()
        {
            HttpClient client = new();

            string fullUrl = this.baseUrl + "SyncTestAppData";

            client.BaseAddress = new Uri(this.baseUrl);

            Dictionary<string, string> data = new()
            {
                { "games",  ds.GetGamesClass().GetGameInfo()},
                { "settings",  ds.GetSettingsClass().GetSettingsInfo()},
                { "tests", ds.GetTestsClass().GetTestInfo()},
                { "questions", ds.GetQuestionsClass().GetQuestionInfo()},
                { "antwoorden", ds.GetAntwoordenClass().GetAntwoordenInfo()},
                { "testVragen", ds.GetTestVragenClass().GetTestVragenInfo()},
                { "opdrachten", ds.GetOpdrachtenClass().GetOpdrachtenInfo()},
                { "testAntwoorden", ds.GetTestAntwoordenClass().GetTestAntwoordenInfo()},
                { "spelers", ds.GetSpelersClass().GetSpelersInfo()}
            };

            string content = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/widmTest";
            string fileName = filePath + "/apisending.txt";
            
            if (!File.Exists(fileName))
            {
                var createdFile = File.Create(fileName);
                createdFile.Close();
            }

            File.WriteAllText(fileName, content);

            var postData = new StringContent(content,Encoding.UTF8, "application/json");

            var response = await client.PostAsync(fullUrl, postData);

            string responseString = await response.Content.ReadAsStringAsync();

            if (responseString.StartsWith("Error"))
                throw new Exception(responseString);

            string fileNameRecieve = filePath + "/apirecieving.txt";

            if (!File.Exists(fileNameRecieve))
            {
                var createdFile = File.Create(fileNameRecieve);
                createdFile.Close();
            }

            File.WriteAllText(fileNameRecieve, responseString);

            Dictionary<string, JArray> returnedObject = (Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,JArray>>(responseString) ?? new()) 
                    ?? throw new Exception("returnObject should not be null here");

            foreach (KeyValuePair<string, JArray> kvp in returnedObject)
            {
                JArray? value = kvp.Value;

                if (value == null)
                    continue;

                switch (kvp.Key)
                {
                    case "antwoorden":
                        if (value.ToObject(objectType: typeof(List<List<string>>)) is not List<List<string>> valuesAntwoorden)
                            continue;

                        ds.GetAntwoordenClass().UpdateFromApi(JsonSerializer.Serialize(valuesAntwoorden));
                        break;
                    case "games":
                        if (value.ToObject(objectType: typeof(List<string>)) is not List<string> valuesGames)
                            continue;

                        ds.GetGamesClass().UpdateFromApi(JsonSerializer.Serialize(valuesGames));
                        break;
                    case "opdrachten":
                        if (value.ToObject(objectType: typeof(List<string>)) is not List<string> valuesOpdrachten)
                            continue;

                        ds.GetOpdrachtenClass().UpdateFromApi(JsonSerializer.Serialize(valuesOpdrachten));
                        break;
                    case "questions":
                        if (value.ToObject(objectType: typeof(List<List<string>>)) is not List<List<string>> valuesQuestions)
                            continue;

                        ds.GetQuestionsClass().UpdateFromApi(JsonSerializer.Serialize(valuesQuestions));
                        break;
                    case "tests":
                        if (value.ToObject(objectType: typeof(List<string>)) is not List<string> valuesTests)
                            continue;

                        ds.GetTestsClass().UpdateFromApi(JsonSerializer.Serialize(valuesTests));
                        break;
                    case "settings":
                        continue;
                    case "testVragen":
                        if (value.ToObject(objectType: typeof(List<List<string>>)) is not List<List<string>> valuesTestVragen)
                            continue;

                        ds.GetTestVragenClass().UpdateFromApi(JsonSerializer.Serialize(valuesTestVragen));
                        break;
                    case "testAntwoorden":
                        if (value.ToObject(objectType: typeof(List<List<string>>)) is not List<List<string>> valuesTestAntwoorden)
                            continue;

                        ds.GetTestAntwoordenClass().UpdateFromApi(JsonSerializer.Serialize(valuesTestAntwoorden));
                        break;
                    case "spelers":
                        if (value.ToObject(objectType: typeof(List<List<string>>)) is not List<List<string>> valuesSpelers)
                            continue;

                        ds.GetSpelersClass().UpdateFromApi(JsonSerializer.Serialize(valuesSpelers));
                        break;

                    default:
                        throw new Exception("Zou hier niet moeten komen!!");
                }
            }
            
        }
    }
}
