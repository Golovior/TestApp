using System.Text.Json;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class LegacyDataImporter
    {
        private static readonly Action<string> Log = message => Debug.WriteLine($"[LegacyImport] {message}");

        private static Guid EnsureTestId(AppDbContext db, string testName)
        {
            Test? localTest = db.Tests.Local.FirstOrDefault(x => x.Name == testName);
            if (localTest != null)
                return localTest.Id;

            Test? test = db.Tests.SingleOrDefault(x => x.Name == testName);
            if (test != null)
                return test.Id;

            test = new Test { Id = Guid.NewGuid(), Name = testName };
            db.Tests.Add(test);
            return test.Id;
        }

        private static Guid EnsureOpdrachtId(AppDbContext db, string opdrachtName)
        {
            Opdracht? localOpdracht = db.Opdrachten.Local.FirstOrDefault(x => x.Name == opdrachtName);
            if (localOpdracht != null)
                return localOpdracht.Id;

            Opdracht? opdracht = db.Opdrachten.SingleOrDefault(x => x.Name == opdrachtName);
            if (opdracht != null)
                return opdracht.Id;

            opdracht = new Opdracht { Id = Guid.NewGuid(), Name = opdrachtName };
            db.Opdrachten.Add(opdracht);
            return opdracht.Id;
        }

        private static Guid EnsureQuestionId(AppDbContext db, string opdrachtName, string questionText)
        {
            Guid opdrachtId = EnsureOpdrachtId(db, opdrachtName);
            Question? localQuestion = db.Questions.Local.FirstOrDefault(x => x.OpdrachtId == opdrachtId && x.Text == questionText);
            if (localQuestion != null)
                return localQuestion.Id;

            Question? question = db.Questions.SingleOrDefault(x => x.OpdrachtId == opdrachtId && x.Text == questionText);
            if (question != null)
                return question.Id;

            question = new Question
            {
                Id = Guid.NewGuid(),
                OpdrachtId = opdrachtId,
                Text = questionText,
                Alphabetical = questionText
            };
            db.Questions.Add(question);
            return question.Id;
        }

        private static Guid EnsurePlayerId(AppDbContext db, string playerName)
        {
            Player? localPlayer = db.Players.Local.FirstOrDefault(x => x.Name == playerName);
            if (localPlayer != null)
                return localPlayer.Id;

            Player? player = db.Players.SingleOrDefault(x => x.Name == playerName);
            if (player != null)
                return player.Id;

            player = new Player
            {
                Id = Guid.NewGuid(),
                Name = playerName
            };
            db.Players.Add(player);
            return player.Id;
        }

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

                db.Games.Add(new Game { Id = Guid.NewGuid(), Name = value });
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

                db.Tests.Add(new Test { Id = Guid.NewGuid(), Name = value });
            }

            if (skipped > 0)
                Log($"tests.txt: skipped {skipped} malformed records.");
        }

        private static void ImportQuestions(AppDbContext db)
        {
            List<List<string>> rows = ReadJson<List<List<string>>>("questions.txt") ?? new();
            HashSet<string> existingKeys = db.Questions
                .AsNoTracking()
                .Select(x => x.OpdrachtId.ToString() + "|" + x.Text)
                .ToHashSet();
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

                Guid opdrachtId = EnsureOpdrachtId(db, opdracht);
                string key = opdrachtId + "|" + text;

                if (existingKeys.Contains(key))
                    continue;

                db.Questions.Add(new Question
                {
                    Id = Guid.NewGuid(),
                    OpdrachtId = opdrachtId,
                    Text = text,
                    Alphabetical = alphabetical
                });
                existingKeys.Add(key);
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

                Guid questionId = EnsureQuestionId(db, opdracht, vraag);

                if (db.Answers.Any(x => x.QuestionId == questionId && x.Name == name))
                    continue;

                db.Answers.Add(new Answer
                {
                    Id = Guid.NewGuid(),
                    QuestionId = questionId,
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

                Guid testId = EnsureTestId(db, row[0]);
                Guid questionId = EnsureQuestionId(db, row[1], row[2]);

                if (db.TestQuestions.Any(x =>
                    x.TestId == testId
                    && x.QuestionId == questionId
                    && x.Order == row[3]))
                    continue;

                db.TestQuestions.Add(new TestQuestion
                {
                    Id = Guid.NewGuid(),
                    TestId = testId,
                    QuestionId = questionId,
                    Order = row[3]
                });
            }

            if (skipped > 0)
                Log($"testvragen.txt: skipped {skipped} malformed records.");
        }

        private static Guid? FindTestQuestionId(AppDbContext db, Guid testId, Guid questionId)
        {
            TestQuestion? local = db.TestQuestions.Local.FirstOrDefault(x => x.TestId == testId && x.QuestionId == questionId);
            if (local != null)
                return local.Id;

            return db.TestQuestions
                .Where(x => x.TestId == testId && x.QuestionId == questionId)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefault();
        }

        private static Guid? FindAnswerId(AppDbContext db, Guid questionId, string answerName)
        {
            Answer? local = db.Answers.Local.FirstOrDefault(x => x.QuestionId == questionId && x.Name == answerName);
            if (local != null)
                return local.Id;

            return db.Answers
                .Where(x => x.QuestionId == questionId && x.Name == answerName)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefault();
        }

        // Legacy testantwoorden.txt rows predate the TestAfname (attempt) concept, so all of a
        // player's legacy answers for a test are grouped into one synthetic attempt. There's no
        // real historical timestamp to recover, so it's stamped with the import time.
        private static Guid EnsureLegacyAfnameId(AppDbContext db, Guid testId, Guid playerId, Dictionary<(Guid, Guid), Guid> afnameCache)
        {
            if (afnameCache.TryGetValue((testId, playerId), out Guid existing))
                return existing;

            TestAfname afname = new()
            {
                Id = Guid.NewGuid(),
                TestId = testId,
                SpelerId = playerId,
                Starttijd = DateTime.Now,
                Eindtijd = DateTime.Now
            };

            db.TestAfnamen.Add(afname);
            afnameCache[(testId, playerId)] = afname.Id;
            return afname.Id;
        }

        private static void ImportTestAntwoorden(AppDbContext db)
        {
            List<List<string>> rows = ReadJson<List<List<string>>>("testantwoorden.txt") ?? new();
            int skipped = 0;
            Dictionary<(Guid, Guid), Guid> afnameCache = new();

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

                Guid testId = EnsureTestId(db, row[0]);
                Guid playerId = EnsurePlayerId(db, row[1]);
                Guid questionId = EnsureQuestionId(db, row[2], row[3]);

                // Rows whose question isn't part of the test (this also filters out the old
                // "einde Test / Tijd gespendeerd" elapsed-time marker rows, which never were a
                // real test question) can't be represented under the new model, and are skipped.
                Guid? testQuestionId = FindTestQuestionId(db, testId, questionId);
                if (!testQuestionId.HasValue)
                {
                    skipped++;
                    continue;
                }

                Guid? answerId = FindAnswerId(db, questionId, row[4]);
                if (!answerId.HasValue)
                {
                    skipped++;
                    continue;
                }

                Guid afnameId = EnsureLegacyAfnameId(db, testId, playerId, afnameCache);

                bool alreadyImported = db.TestAnswers.Local.Any(x =>
                        x.TestAfnameId == afnameId && x.TestQuestionId == testQuestionId.Value)
                    || db.TestAnswers.Any(x =>
                        x.TestAfnameId == afnameId && x.TestQuestionId == testQuestionId.Value);

                if (alreadyImported)
                    continue;

                db.TestAnswers.Add(new TestAnswer
                {
                    Id = Guid.NewGuid(),
                    TestAfnameId = afnameId,
                    TestQuestionId = testQuestionId.Value,
                    AnswerId = answerId.Value
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

                db.Opdrachten.Add(new Opdracht { Id = Guid.NewGuid(), Name = value });
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

                // row[1] was the legacy global active/inactive flag; there's no per-game
                // home to migrate it into, so it's intentionally dropped on import.
                db.Players.Add(new Player
                {
                    Id = Guid.NewGuid(),
                    Name = row[0]
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