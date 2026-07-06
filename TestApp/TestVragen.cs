using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class TestVragen : ITestVragenStore
    {
        public TestVragen() { }

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

        public bool TestIsAfgenomen(string test)
        {
            using AppDbContext db = new();
            return db.TestAfnamen.Any(x => x.Test.Name == test);
        }

        public bool QuestionAlreadyExists(string test, string opdracht, string question)
        {
            using AppDbContext db = new();
            Guid? testId = db.Tests
                .Where(x => x.Name == test)
                .Select(x => (Guid?)x.Id)
                .SingleOrDefault();

            if (!testId.HasValue)
                return false;

            return db.TestQuestions.Any(x =>
                x.TestId == testId.Value
                && x.Question.Opdracht.Name == opdracht
                && x.Question.Text == question);
        }

        public List<List<string>> GetAllTestVragen()
        {
            using AppDbContext db = new();
            return db.TestQuestions
                .AsNoTracking()
                .Include(x => x.Test)
                .Include(x => x.Question)
                .ThenInclude(x => x.Opdracht)
                .Select(x => new List<string>
                {
                    x.Test.Name,
                    x.Question.Opdracht.Name,
                    x.Question.Text,
                    x.Order
                })
                .ToList();
        }

        public void AddTestVraag(string test, string opdracht, string question, string order)
        {
            using AppDbContext db = new();
            Guid testId = EnsureTestId(db, test);

            if (db.TestAfnamen.Any(x => x.TestId == testId))
                return;

            Guid questionId = EnsureQuestionId(db, opdracht, question);
            db.TestQuestions.Add(new TestQuestion
            {
                Id = Guid.NewGuid(),
                TestId = testId,
                QuestionId = questionId,
                Order = order
            });
            RecordSyncHelper.TouchRecordTimestamp(db, "testVragen", new[] { test, opdracht, question, order });
            db.SaveChanges();
        }

        public void RemoveTestVragen(List<string> vraag)
        {
            if (vraag.Count < 4)
                return;

            using AppDbContext db = new();
            Guid? testId = db.Tests
                .Where(x => x.Name == vraag[0])
                .Select(x => (Guid?)x.Id)
                .SingleOrDefault();

            if (!testId.HasValue)
                return;

            if (db.TestAfnamen.Any(x => x.TestId == testId.Value))
                return;

            TestQuestion? current = db.TestQuestions.FirstOrDefault(x =>
                x.TestId == testId.Value
                && x.Question.Opdracht.Name == vraag[1]
                && x.Question.Text == vraag[2]
                && x.Order == vraag[3]);

            if (current != null)
            {
                db.TestQuestions.Remove(current);
                RecordSyncHelper.TouchRecordTimestamp(db, "testVragen", new[] { vraag[0], vraag[1], vraag[2], vraag[3] });
                db.SaveChanges();
            }
        }

        public string GetTestVragenInfo()
        {
            List<List<string>> appTestVragen = GetAllTestVragen();
            return JsonSerializer.Serialize(appTestVragen);
        }

        public void SaveTestVragen()
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
                if (row.Count < 4)
                    continue;

                string[] keyParts = { row[0], row[1], row[2], row[3] };
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "testVragen", keyParts, remoteTimestamp))
                    continue;

                Guid testId = EnsureTestId(db, row[0]);
                Guid questionId = EnsureQuestionId(db, row[1], row[2]);

                TestQuestion? current = db.TestQuestions.SingleOrDefault(x =>
                    x.TestId == testId && x.QuestionId == questionId && x.Order == row[3]);

                if (current == null)
                {
                    db.TestQuestions.Add(new TestQuestion
                    {
                        Id = Guid.NewGuid(),
                        TestId = testId,
                        QuestionId = questionId,
                        Order = row[3]
                    });
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "testVragen", keyParts, remoteTimestamp);
            }

            db.SaveChanges();
            transaction.Commit();
        }

    }
}

