using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Test? test = db.Tests.SingleOrDefault(x => x.Name == testName);
            if (test != null)
                return test.Id;

            test = new Test { Id = Guid.NewGuid(), Name = testName };
            db.Tests.Add(test);
            db.SaveChanges();
            return test.Id;
        }

        private static Guid EnsurePlayerId(AppDbContext db, string speler)
        {
            Player? player = db.Players.SingleOrDefault(x => x.Name == speler);
            if (player != null)
                return player.Id;

            player = new Player
            {
                Id = Guid.NewGuid(),
                Name = speler,
                Status = "1"
            };
            db.Players.Add(player);
            db.SaveChanges();
            return player.Id;
        }

        private static Guid EnsureQuestionId(AppDbContext db, string opdracht, string question)
        {
            Opdracht? opdrachtEntity = db.Opdrachten.SingleOrDefault(x => x.Name == opdracht);
            if (opdrachtEntity == null)
            {
                opdrachtEntity = new Opdracht { Id = Guid.NewGuid(), Name = opdracht };
                db.Opdrachten.Add(opdrachtEntity);
                db.SaveChanges();
            }

            Question? questionEntity = db.Questions.SingleOrDefault(x => x.OpdrachtId == opdrachtEntity.Id && x.Text == question);
            if (questionEntity != null)
                return questionEntity.Id;

            questionEntity = new Question
            {
                Id = Guid.NewGuid(),
                OpdrachtId = opdrachtEntity.Id,
                Text = question,
                Alphabetical = question
            };
            db.Questions.Add(questionEntity);
            db.SaveChanges();
            return questionEntity.Id;
        }

        private static Guid? TryGetAnswerId(AppDbContext db, Guid questionId, string answerText)
        {
            return db.Answers
                .Where(x => x.QuestionId == questionId && x.Name == answerText)
                .Select(x => (Guid?)x.Id)
                .SingleOrDefault();
        }

        public bool AntwoordAlreadyExists(string test, string speler, string opdracht, string question, string antwoord)
        {
            using AppDbContext db = new();
            Guid? testId = db.Tests
                .Where(x => x.Name == test)
                .Select(x => (Guid?)x.Id)
                .SingleOrDefault();

            if (!testId.HasValue)
                return false;

            return db.TestAnswers.Any(x =>
                x.TestId == testId.Value
                && x.Player.Name == speler
                && x.PlayerId == x.Player.Id
                && x.QuestionText == question
                && x.QuestionText == question
                && x.AnswerText == antwoord);
        }

        public List<List<string>> GetAllTestAntwoorden()
        {
            using AppDbContext db = new();
            return db.TestAnswers
                .AsNoTracking()
                .Include(x => x.Test)
                .Include(x => x.Player)
                .Include(x => x.Answer)
                .ThenInclude(x => x!.Question)
                .ThenInclude(x => x.Opdracht)
                .Select(x => new List<string>
                {
                    x.Test.Name,
                    x.Player.Name,
                    x.Answer != null ? x.Answer.Question.Opdracht.Name : string.Empty,
                    x.QuestionText,
                    x.AnswerText
                })
                .ToList();
        }

        public void AddTestAntwoord(string test, string speler, string opdracht, string question, string antwoord)
        {
            using AppDbContext db = new();
            Guid testId = EnsureTestId(db, test);
            Guid playerId = EnsurePlayerId(db, speler);
            Guid questionId = EnsureQuestionId(db, opdracht, question);
            Guid? answerId = TryGetAnswerId(db, questionId, antwoord);
            db.TestAnswers.Add(new TestAnswer
            {
                Id = Guid.NewGuid(),
                TestId = testId,
                PlayerId = playerId,
                AnswerId = answerId,
                QuestionText = question,
                AnswerText = antwoord
            });
            RecordSyncHelper.TouchRecordTimestamp(db, "testAntwoorden", new[] { test, speler, opdracht, question, antwoord });
            db.SaveChanges();
        }

        public void RemoveTestAntwoord(List<string> antwoord)
        {
            if (antwoord.Count < 5)
                return;

            using AppDbContext db = new();
            Guid? testId = db.Tests
                .Where(x => x.Name == antwoord[0])
                .Select(x => (Guid?)x.Id)
                .SingleOrDefault();

            if (!testId.HasValue)
                return;

            Guid? playerId = db.Players.Where(x => x.Name == antwoord[1]).Select(x => (Guid?)x.Id).SingleOrDefault();
            if (!playerId.HasValue)
                return;

            TestAnswer? current = db.TestAnswers.FirstOrDefault(x =>
                x.TestId == testId.Value
                && x.PlayerId == playerId.Value
                && x.QuestionText == antwoord[3]
                && x.AnswerText == antwoord[4]);

            if (current != null)
            {
                db.TestAnswers.Remove(current);
                RecordSyncHelper.TouchRecordTimestamp(db, "testAntwoorden", new[] { antwoord[0], antwoord[1], antwoord[2], antwoord[3], antwoord[4] });
                db.SaveChanges();
            }
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
                Guid playerId = EnsurePlayerId(db, row[1]);
                Guid questionId = EnsureQuestionId(db, row[2], row[3]);
                Guid? answerId = TryGetAnswerId(db, questionId, row[4]);

                TestAnswer? current = db.TestAnswers.SingleOrDefault(x =>
                    x.TestId == testId
                    && x.PlayerId == playerId
                    && x.QuestionText == row[3]
                    && x.AnswerText == row[4]);

                if (current == null)
                {
                    db.TestAnswers.Add(new TestAnswer
                    {
                        Id = Guid.NewGuid(),
                        TestId = testId,
                        PlayerId = playerId,
                        AnswerId = answerId,
                        QuestionText = row[3],
                        AnswerText = row[4]
                    });
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "testAntwoorden", keyParts, remoteTimestamp);
            }

            db.SaveChanges();
            transaction.Commit();
        }

    }
}

