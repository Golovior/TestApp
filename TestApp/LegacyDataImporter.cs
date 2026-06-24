using System.Text.Json;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class LegacyDataImporter
    {
        private static readonly Action<string> Log = message => Debug.WriteLine($"[LegacyImport] {message}");

        public void ImportFromLegacyFiles()
        {
            using AppDbContext db = new();

            ImportGames(db);
            ImportSettings(db);
            ImportTests(db);
            ImportQuestions(db);
            ImportAnswers(db);
            ImportTestVragen(db);
            ImportTestAntwoorden(db);
            ImportOpdrachten(db);
            ImportSpelers(db);

            db.SaveChanges();
        }

        private static void ImportGames(AppDbContext db)
        {
            List<string> rows = ReadJson<List<string>>("games.txt") ?? new();
            int skipped = 0;
            foreach (string value in rows)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    skipped++;
                    continue;
                }

                if (db.Games.Any(x => x.Name == value))
                    continue;

                db.Games.Add(new Game { Name = value });
            }

            if (skipped > 0)
                Log($"games.txt: skipped {skipped} malformed records.");
        }

        private static void ImportSettings(AppDbContext db)
        {
            List<KeyValuePair<string, string>> rows = ReadJson<List<KeyValuePair<string, string>>>("settings.txt") ?? new();
            int skipped = 0;
            foreach (KeyValuePair<string, string> row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.Key) || row.Value == null)
                {
                    skipped++;
                    continue;
                }

                SettingEntry? current = db.Settings.SingleOrDefault(x => x.Key == row.Key);
                if (current == null)
                {
                    db.Settings.Add(new SettingEntry
                    {
                        Key = row.Key,
                        Value = row.Value
                    });
                }
                else
                {
                    current.Value = row.Value;
                }
            }

            if (skipped > 0)
                Log($"settings.txt: skipped {skipped} malformed records.");
        }

        private static void ImportTests(AppDbContext db)
        {
            List<string> rows = ReadJson<List<string>>("tests.txt") ?? new();
            int skipped = 0;
            foreach (string value in rows)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    skipped++;
                    continue;
                }

                if (db.Tests.Any(x => x.Name == value))
                    continue;

                db.Tests.Add(new Test { Name = value });
            }

            if (skipped > 0)
                Log($"tests.txt: skipped {skipped} malformed records.");
        }

        private static void ImportQuestions(AppDbContext db)
        {
            List<List<string>> rows = ReadJson<List<List<string>>>("questions.txt") ?? new();
            int skipped = 0;
            foreach (List<string> row in rows)
            {
                if (row.Count < 3)
                {
                    skipped++;
                    continue;
                }

                string opdracht = row[0];
                string text = row[1];
                string alphabetical = row[2];

                if (string.IsNullOrWhiteSpace(opdracht) || string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(alphabetical))
                {
                    skipped++;
                    continue;
                }

                if (db.Questions.Any(x => x.Opdracht == opdracht && x.Text == text))
                    continue;

                db.Questions.Add(new Question
                {
                    Opdracht = opdracht,
                    Text = text,
                    Alphabetical = alphabetical
                });
            }

            if (skipped > 0)
                Log($"questions.txt: skipped {skipped} malformed records.");
        }

        private static void ImportAnswers(AppDbContext db)
        {
            List<List<string>> rows = ReadJson<List<List<string>>>("antwoorden.txt") ?? new();
            int skipped = 0;
            foreach (List<string> row in rows)
            {
                if (row.Count < 5)
                {
                    skipped++;
                    continue;
                }

                string opdracht = row[0];
                string vraag = row[1];
                string name = row[2];

                if (string.IsNullOrWhiteSpace(opdracht) || string.IsNullOrWhiteSpace(vraag) || string.IsNullOrWhiteSpace(name))
                {
                    skipped++;
                    continue;
                }

                if (db.Answers.Any(x => x.Opdracht == opdracht && x.Vraag == vraag && x.Name == name))
                    continue;

                db.Answers.Add(new Answer
                {
                    Opdracht = opdracht,
                    Vraag = vraag,
                    Name = name,
                    IsCorrect = row[3] == "1",
                    ConnectedPlayersJson = string.IsNullOrWhiteSpace(row[4]) ? "[]" : row[4]
                });
            }

            if (skipped > 0)
                Log($"antwoorden.txt: skipped {skipped} malformed records.");
        }

        private static void ImportTestVragen(AppDbContext db)
        {
            List<List<string>> rows = ReadJson<List<List<string>>>("testvragen.txt") ?? new();
            int skipped = 0;
            foreach (List<string> row in rows)
            {
                if (row.Count < 4)
                {
                    skipped++;
                    continue;
                }

                if (row.Any(string.IsNullOrWhiteSpace))
                {
                    skipped++;
                    continue;
                }

                if (db.TestQuestions.Any(x =>
                    x.TestName == row[0]
                    && x.Opdracht == row[1]
                    && x.QuestionText == row[2]
                    && x.Order == row[3]))
                    continue;

                db.TestQuestions.Add(new TestQuestion
                {
                    TestName = row[0],
                    Opdracht = row[1],
                    QuestionText = row[2],
                    Order = row[3]
                });
            }

            if (skipped > 0)
                Log($"testvragen.txt: skipped {skipped} malformed records.");
        }

        private static void ImportTestAntwoorden(AppDbContext db)
        {
            List<List<string>> rows = ReadJson<List<List<string>>>("testantwoorden.txt") ?? new();
            int skipped = 0;
            foreach (List<string> row in rows)
            {
                if (row.Count < 5)
                {
                    skipped++;
                    continue;
                }

                if (row.Any(string.IsNullOrWhiteSpace))
                {
                    skipped++;
                    continue;
                }

                if (db.TestAnswers.Any(x =>
                    x.TestName == row[0]
                    && x.Speler == row[1]
                    && x.Opdracht == row[2]
                    && x.QuestionText == row[3]
                    && x.AnswerText == row[4]))
                    continue;

                db.TestAnswers.Add(new TestAnswer
                {
                    TestName = row[0],
                    Speler = row[1],
                    Opdracht = row[2],
                    QuestionText = row[3],
                    AnswerText = row[4]
                });
            }

            if (skipped > 0)
                Log($"testantwoorden.txt: skipped {skipped} malformed records.");
        }

        private static void ImportOpdrachten(AppDbContext db)
        {
            List<string> rows = ReadJson<List<string>>("opdrachten.txt") ?? new();
            int skipped = 0;
            foreach (string value in rows)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    skipped++;
                    continue;
                }

                if (db.Opdrachten.Any(x => x.Name == value))
                    continue;

                db.Opdrachten.Add(new Opdracht { Name = value });
            }

            if (skipped > 0)
                Log($"opdrachten.txt: skipped {skipped} malformed records.");
        }

        private static void ImportSpelers(AppDbContext db)
        {
            List<List<string>> rows = ReadJson<List<List<string>>>("spelers.txt") ?? new();
            int skipped = 0;
            foreach (List<string> row in rows)
            {
                if (row.Count < 2)
                {
                    skipped++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(row[0]) || string.IsNullOrWhiteSpace(row[1]))
                {
                    skipped++;
                    continue;
                }

                if (db.Players.Any(x => x.Name == row[0]))
                    continue;

                db.Players.Add(new Player
                {
                    Name = row[0],
                    Status = row[1]
                });
            }

            if (skipped > 0)
                Log($"spelers.txt: skipped {skipped} malformed records.");
        }

        private static T? ReadJson<T>(string fileName)
        {
            string fullPath = Path.Combine(AppStoragePaths.DataDirectory, fileName);
            if (!File.Exists(fullPath))
                return default;

            string text = File.ReadAllText(fullPath);
            if (string.IsNullOrWhiteSpace(text))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(text);
            }
            catch (JsonException)
            {
                Log($"{fileName}: invalid JSON, file ignored.");
                return default;
            }
        }
    }
}