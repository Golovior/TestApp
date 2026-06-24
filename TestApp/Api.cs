using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TestApp
{
    internal class Api
    {
        private static readonly HttpClient HttpClient = new();
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        private sealed class ApiSyncRequestDto
        {
            public string games { get; set; } = "[]";
            public string settings { get; set; } = "[]";
            public string tests { get; set; } = "[]";
            public string questions { get; set; } = "[]";
            public string antwoorden { get; set; } = "[]";
            public string testVragen { get; set; } = "[]";
            public string opdrachten { get; set; } = "[]";
            public string testAntwoorden { get; set; } = "[]";
            public string spelers { get; set; } = "[]";
        }

        private sealed class ApiSyncResponseDto
        {
            public List<string>? games { get; set; }
            public List<string>? settings { get; set; }
            public List<string>? tests { get; set; }
            public List<List<string>>? questions { get; set; }
            public List<List<string>>? antwoorden { get; set; }
            public List<List<string>>? testVragen { get; set; }
            public List<string>? opdrachten { get; set; }
            public List<List<string>>? testAntwoorden { get; set; }
            public List<List<string>>? spelers { get; set; }

            public long? gamesTimestampUtc { get; set; }
            public long? settingsTimestampUtc { get; set; }
            public long? testsTimestampUtc { get; set; }
            public long? questionsTimestampUtc { get; set; }
            public long? antwoordenTimestampUtc { get; set; }
            public long? testVragenTimestampUtc { get; set; }
            public long? opdrachtenTimestampUtc { get; set; }
            public long? testAntwoordenTimestampUtc { get; set; }
            public long? spelersTimestampUtc { get; set; }
        }

        readonly string baseUrl;
        readonly DataSetClass ds;


        private static long? GetRemoteTimestampFromPayload(JObject payloadObject, string tableName)
        {
            string[] timestampKeys =
            {
                $"{tableName}TimestampUtc",
                $"{tableName}Timestamp",
                $"{tableName}UpdatedAtUtc",
                $"{tableName}UpdatedAt"
            };

            foreach (string key in timestampKeys)
            {
                JToken? token = payloadObject[key];
                if (token == null)
                    continue;

                if (token.Type == JTokenType.Integer && token.Value<long?>() is long intValue)
                    return intValue;

                if (token.Type == JTokenType.String && long.TryParse(token.Value<string>(), out long parsedStringValue))
                    return parsedStringValue;
            }

            return null;
        }

        private static void ApplyRemoteTable(string tableName, string serializedRows, long? remoteTimestamp, Action<string, long?> updateAction)
        {
            updateAction(serializedRows, remoteTimestamp);
        }

        public Api(DataSetClass ds) {
            this.baseUrl = "http://widmtimer.fvandenberg.nl/api/";

            this.ds = ds;
        }

        public async Task SaveData()
        {
            string fullUrl = this.baseUrl + "SyncTestAppData";

            ApiSyncRequestDto data = new()
            {
                games = ds.GetGamesClass().GetGameInfo(),
                settings = ds.GetSettingsClass().GetSettingsInfo(),
                tests = ds.GetTestsClass().GetTestInfo(),
                questions = ds.GetQuestionsClass().GetQuestionInfo(),
                antwoorden = ds.GetAntwoordenClass().GetAntwoordenInfo(),
                testVragen = ds.GetTestVragenClass().GetTestVragenInfo(),
                opdrachten = ds.GetOpdrachtenClass().GetOpdrachtenInfo(),
                testAntwoorden = ds.GetTestAntwoordenClass().GetTestAntwoordenInfo(),
                spelers = ds.GetSpelersClass().GetSpelersInfo()
            };

            string content = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            string filePath = AppStoragePaths.DataDirectory;
            string fileName = Path.Combine(filePath, "apisending.txt");
            
            if (!File.Exists(fileName))
            {
                var createdFile = File.Create(fileName);
                createdFile.Close();
            }

            File.WriteAllText(fileName, content);

            var postData = new StringContent(content,Encoding.UTF8, "application/json");

            string responseString;

            try
            {
                using var cts = new CancellationTokenSource(DefaultTimeout);
                var response = await HttpClient.PostAsync(fullUrl, postData, cts.Token);
                response.EnsureSuccessStatusCode();

                responseString = await response.Content.ReadAsStringAsync(cts.Token);
            }
            catch (OperationCanceledException ex)
            {
                throw new Exception("API sync timed out while sending data.", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API sync failed: {ex.Message}", ex);
            }

            if (responseString.StartsWith("Error", StringComparison.OrdinalIgnoreCase))
                throw new Exception($"API sync returned an error: {responseString}");

            string fileNameRecieve = Path.Combine(filePath, "apirecieving.txt");

            if (!File.Exists(fileNameRecieve))
            {
                var createdFile = File.Create(fileNameRecieve);
                createdFile.Close();
            }

            File.WriteAllText(fileNameRecieve, responseString);

            ApiSyncResponseDto returnedObject;
            JObject responseObject;

            try
            {
                returnedObject = (Newtonsoft.Json.JsonConvert.DeserializeObject<ApiSyncResponseDto>(responseString) ?? new())
                    ?? throw new Exception("returnObject should not be null here");
                responseObject = JObject.Parse(responseString);
            }
            catch (Exception ex)
            {
                throw new Exception("API sync returned invalid JSON payload.", ex);
            }

            if (returnedObject.antwoorden != null)
            {
                long? antwoordenTimestamp = returnedObject.antwoordenTimestampUtc ?? GetRemoteTimestampFromPayload(responseObject, "antwoorden");
                ApplyRemoteTable(
                    "antwoorden",
                    Newtonsoft.Json.JsonConvert.SerializeObject(returnedObject.antwoorden),
                    antwoordenTimestamp,
                    (serializedRows, timestamp) => ds.GetAntwoordenClass().UpdateFromApi(serializedRows, timestamp));
            }

            if (returnedObject.games != null)
            {
                long? gamesTimestamp = returnedObject.gamesTimestampUtc ?? GetRemoteTimestampFromPayload(responseObject, "games");
                ApplyRemoteTable(
                    "games",
                    Newtonsoft.Json.JsonConvert.SerializeObject(returnedObject.games),
                    gamesTimestamp,
                    (serializedRows, timestamp) => ds.GetGamesClass().UpdateFromApi(serializedRows, timestamp));
            }

            if (returnedObject.opdrachten != null)
            {
                long? opdrachtenTimestamp = returnedObject.opdrachtenTimestampUtc ?? GetRemoteTimestampFromPayload(responseObject, "opdrachten");
                ApplyRemoteTable(
                    "opdrachten",
                    Newtonsoft.Json.JsonConvert.SerializeObject(returnedObject.opdrachten),
                    opdrachtenTimestamp,
                    (serializedRows, timestamp) => ds.GetOpdrachtenClass().UpdateFromApi(serializedRows, timestamp));
            }

            if (returnedObject.questions != null)
            {
                long? questionsTimestamp = returnedObject.questionsTimestampUtc ?? GetRemoteTimestampFromPayload(responseObject, "questions");
                ApplyRemoteTable(
                    "questions",
                    Newtonsoft.Json.JsonConvert.SerializeObject(returnedObject.questions),
                    questionsTimestamp,
                    (serializedRows, timestamp) => ds.GetQuestionsClass().UpdateFromApi(serializedRows, timestamp));
            }

            if (returnedObject.tests != null)
            {
                long? testsTimestamp = returnedObject.testsTimestampUtc ?? GetRemoteTimestampFromPayload(responseObject, "tests");
                ApplyRemoteTable(
                    "tests",
                    Newtonsoft.Json.JsonConvert.SerializeObject(returnedObject.tests),
                    testsTimestamp,
                    (serializedRows, timestamp) => ds.GetTestsClass().UpdateFromApi(serializedRows, timestamp));
            }

            if (returnedObject.testVragen != null)
            {
                long? testVragenTimestamp = returnedObject.testVragenTimestampUtc ?? GetRemoteTimestampFromPayload(responseObject, "testVragen");
                ApplyRemoteTable(
                    "testVragen",
                    Newtonsoft.Json.JsonConvert.SerializeObject(returnedObject.testVragen),
                    testVragenTimestamp,
                    (serializedRows, timestamp) => ds.GetTestVragenClass().UpdateFromApi(serializedRows, timestamp));
            }

            if (returnedObject.testAntwoorden != null)
            {
                long? testAntwoordenTimestamp = returnedObject.testAntwoordenTimestampUtc ?? GetRemoteTimestampFromPayload(responseObject, "testAntwoorden");
                ApplyRemoteTable(
                    "testAntwoorden",
                    Newtonsoft.Json.JsonConvert.SerializeObject(returnedObject.testAntwoorden),
                    testAntwoordenTimestamp,
                    (serializedRows, timestamp) => ds.GetTestAntwoordenClass().UpdateFromApi(serializedRows, timestamp));
            }

            if (returnedObject.spelers != null)
            {
                long? spelersTimestamp = returnedObject.spelersTimestampUtc ?? GetRemoteTimestampFromPayload(responseObject, "spelers");
                ApplyRemoteTable(
                    "spelers",
                    Newtonsoft.Json.JsonConvert.SerializeObject(returnedObject.spelers),
                    spelersTimestamp,
                    (serializedRows, timestamp) => ds.GetSpelersClass().UpdateFromApi(serializedRows, timestamp));
            }
            
        }
    }
}
