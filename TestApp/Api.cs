using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async void SaveData()
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

            string content = JsonConvert.SerializeObject(data);

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

        }
    }
}
