using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class TestAntwoorden : ITestAntwoordenStore
    {
        public TestAntwoorden() { }

        private static Guid EnsureTestId(AppDbContext db, string testName)
        {
            Test? test = db.Tests
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .FirstOrDefault(x => x.Name == testName);
            if (test != null)
                return test.Id;

            test = new Test { Id = Guid.NewGuid(), Name = testName };
            db.Tests.Add(test);
            db.SaveChanges();
            return test.Id;
        }

        private static Guid EnsurePlayerId(AppDbContext db, string speler)
        {
            Player? player = db.Players
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .FirstOrDefault(x => x.Name == speler);
            if (player != null)
                return player.Id;

            player = new Player
            {
                Id = Guid.NewGuid(),
                Name = speler
            };
            db.Players.Add(player);
            db.SaveChanges();
            return player.Id;
        }

        // Resolves the specific TestQuestion + Answer for (opdracht, question, antwoord) within
        // the test that the given TestAfname belongs to, and records the answer if both are
        // found and this question hasn't already been answered in this attempt. Returns false
        // (no-op) if the question isn't part of the test, the answer text doesn't match a real
        // Answer for that question, or the question was already answered in this attempt.
        private static bool TryAddCore(AppDbContext db, Guid testAfnameId, string opdracht, string question, string antwoord)
        {
            TestAfname? afname = db.TestAfnamen.Find(testAfnameId);
            if (afname == null)
                return false;

            TestQuestion? testQuestion = db.TestQuestions
                .Where(tq => tq.TestId == afname.TestId
                    && tq.Question.Opdracht.Name == opdracht
                    && tq.Question.Text == question)
                .OrderBy(tq => tq.Id)
                .FirstOrDefault();

            if (testQuestion == null)
                return false;

            Guid? answerId = db.Answers
                .Where(a => a.QuestionId == testQuestion.QuestionId && a.Name == antwoord)
                .Select(a => (Guid?)a.Id)
                .OrderBy(x => x)
                .FirstOrDefault();

            if (!answerId.HasValue)
                return false;

            bool alreadyAnswered = db.TestAnswers.Any(x =>
                x.TestAfnameId == testAfnameId && x.TestQuestionId == testQuestion.Id);

            if (alreadyAnswered)
                return false;

            db.TestAnswers.Add(new TestAnswer
            {
                Id = Guid.NewGuid(),
                TestAfnameId = testAfnameId,
                TestQuestionId = testQuestion.Id,
                AnswerId = answerId.Value
            });

            RecordSyncHelper.TouchRecordTimestamp(db, "testAntwoorden", new[] { testAfnameId.ToString(), testQuestion.Id.ToString() });
            db.SaveChanges();
            return true;
        }

        private static Guid FindOrCreateSyncAfname(AppDbContext db, Guid testId, Guid spelerId)
        {
            TestAfname? afname = db.TestAfnamen
                .Where(x => x.TestId == testId && x.SpelerId == spelerId)
                .OrderByDescending(x => x.Starttijd)
                .FirstOrDefault();

            if (afname != null)
                return afname.Id;

            afname = new TestAfname
            {
                Id = Guid.NewGuid(),
                TestId = testId,
                SpelerId = spelerId,
                Starttijd = DateTime.Now,
                Eindtijd = DateTime.Now
            };

            db.TestAfnamen.Add(afname);
            db.SaveChanges();
            return afname.Id;
        }

        public bool TryAddTestAntwoord(Guid testAfnameId, string opdracht, string question, string antwoord)
        {
            using AppDbContext db = new();
            return TryAddCore(db, testAfnameId, opdracht, question, antwoord);
        }

        public List<List<string>> GetAllTestAntwoorden()
        {
            using AppDbContext db = new();
            return db.TestAnswers
                .AsNoTracking()
                .Include(x => x.TestAfname).ThenInclude(a => a.Test)
                .Include(x => x.TestAfname).ThenInclude(a => a.Speler)
                .Include(x => x.TestQuestion).ThenInclude(q => q.Question).ThenInclude(q => q.Opdracht)
                .Include(x => x.Answer)
                .Select(x => new List<string>
                {
                    x.TestAfname.Test.Name,
                    x.TestAfname.Speler.Name,
                    x.TestQuestion.Question.Opdracht.Name,
                    x.TestQuestion.Question.Text,
                    x.Answer.Name
                })
                .ToList();
        }

        public string GetTestAntwoordenInfo()
        {
            List<List<string>> appTestAntwoorden = GetAllTestAntwoorden();
            return JsonSerializer.Serialize(appTestAntwoorden);
        }

        public void SaveTestAntwoorden()
        {
            // Persisted directly on each mutating operation.
        }

        public void UpdateFromApi(string data, long? remoteTimestamp = null)
        {
            List<List<string>> rows = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(data) ?? new();

            using AppDbContext db = new();
            using var transaction = db.Database.BeginTransaction();

            foreach (List<string> row in rows)
            {
                if (row.Count < 5)
                    continue;

                string[] keyParts = { row[0], row[1], row[2], row[3], row[4] };
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "testAntwoorden", keyParts, remoteTimestamp))
                    continue;

                Guid testId = EnsureTestId(db, row[0]);
                Guid spelerId = EnsurePlayerId(db, row[1]);
                Guid testAfnameId = FindOrCreateSyncAfname(db, testId, spelerId);

                TryAddCore(db, testAfnameId, row[2], row[3], row[4]);

                RecordSyncHelper.TouchRecordTimestamp(db, "testAntwoorden", keyParts, remoteTimestamp);
            }

            db.SaveChanges();
            transaction.Commit();
        }

    }
}
