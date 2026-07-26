using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Api
    {
        private static readonly HttpClient HttpClient = new();
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        readonly string baseUrl;
        readonly DataSetClass ds;

        public Api(DataSetClass ds) {
            this.baseUrl = "http://widmtimer.fvandenberg.nl/api/";

            this.ds = ds;
        }

        private SyncEnvelope BuildOutgoingEnvelope()
        {
            return new SyncEnvelope
            {
                Games = ds.GetGamesClass().GetForSync(),
                Settings = ds.GetSettingsClass().GetForSync(),
                Opdrachten = ds.GetOpdrachtenClass().GetForSync(),
                Questions = ds.GetQuestionsClass().GetForSync(),
                Answers = ds.GetAntwoordenClass().GetForSync(),
                Tests = ds.GetTestsClass().GetForSync(),
                TestQuestions = ds.GetTestVragenClass().GetForSync(),
                Players = ds.GetSpelersClass().GetForSync(),
                GameSpelers = ds.GetGameSpelersClass().GetForSync(),
                TestAfnamen = ds.GetTestAfnamenClass().GetForSync(),
                TestAnswers = ds.GetTestAntwoordenClass().GetForSync()
            };
        }

        // Parents must be applied before children so foreign keys resolve:
        // (no deps) -> Questions/Tests -> Answers/TestQuestions/GameSpelers -> TestAfnamen -> TestAnswers.
        private void ApplyIncomingEnvelope(SyncEnvelope envelope)
        {
            ds.GetSettingsClass().ApplyFromSync(envelope.Settings);
            ds.GetOpdrachtenClass().ApplyFromSync(envelope.Opdrachten);
            ds.GetGamesClass().ApplyFromSync(envelope.Games);
            ds.GetSpelersClass().ApplyFromSync(envelope.Players);

            ds.GetQuestionsClass().ApplyFromSync(envelope.Questions);
            ds.GetTestsClass().ApplyFromSync(envelope.Tests);

            ds.GetAntwoordenClass().ApplyFromSync(envelope.Answers);
            ds.GetTestVragenClass().ApplyFromSync(envelope.TestQuestions);
            ds.GetGameSpelersClass().ApplyFromSync(envelope.GameSpelers);

            ds.GetTestAfnamenClass().ApplyFromSync(envelope.TestAfnamen);

            ds.GetTestAntwoordenClass().ApplyFromSync(envelope.TestAnswers);
        }

        public async Task SaveData()
        {
            string fullUrl = this.baseUrl + "SyncTestAppData";

            SyncEnvelope outgoing = BuildOutgoingEnvelope();
            string content = JsonSerializer.Serialize(outgoing);

            string filePath = AppStoragePaths.DataDirectory;
            string fileName = Path.Combine(filePath, "apisending.txt");

            if (!File.Exists(fileName))
            {
                var createdFile = File.Create(fileName);
                createdFile.Close();
            }

            File.WriteAllText(fileName, content);

            var postData = new StringContent(content, Encoding.UTF8, "application/json");

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

            SyncEnvelope incoming;

            try
            {
                incoming = JsonSerializer.Deserialize<SyncEnvelope>(responseString) ?? new();
            }
            catch (Exception ex)
            {
                throw new Exception("API sync returned invalid JSON payload.", ex);
            }

            ApplyIncomingEnvelope(incoming);
        }
    }
}
