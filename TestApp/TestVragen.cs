using System;
using System.Collections.Generic;
using System.Linq;
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
            Guid id = Guid.NewGuid();
            db.TestQuestions.Add(new TestQuestion
            {
                Id = id,
                TestId = testId,
                QuestionId = questionId,
                Order = order
            });
            RecordSyncHelper.TouchRecordTimestamp(db, "testVragen", id.ToString());
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
                RecordSyncHelper.TouchRecordTimestamp(db, "testVragen", current.Id.ToString());
                db.TestQuestions.Remove(current);
                db.SaveChanges();
            }
        }

        public void SaveTestVragen()
        {
            // Persisted directly on each mutating operation.
        }

        public List<TestQuestionSyncDto> GetForSync()
        {
            using AppDbContext db = new();
            List<TestQuestionSyncDto> rows = db.TestQuestions.AsNoTracking().Select(x => new TestQuestionSyncDto
            {
                Id = x.Id,
                TestId = x.TestId,
                QuestionId = x.QuestionId,
                Order = x.Order
            }).ToList();

            foreach (TestQuestionSyncDto row in rows)
                row.UpdatedAtUtc = RecordSyncHelper.GetRecordTimestamp(db, "testVragen", row.Id.ToString()) ?? 0;

            return rows;
        }

        public void ApplyFromSync(List<TestQuestionSyncDto> rows)
        {
            using AppDbContext db = new();

            foreach (TestQuestionSyncDto row in rows)
            {
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "testVragen", row.Id.ToString(), row.UpdatedAtUtc))
                    continue;

                TestQuestion? existing = db.TestQuestions.Find(row.Id);
                if (existing == null)
                {
                    db.TestQuestions.Add(new TestQuestion
                    {
                        Id = row.Id,
                        TestId = row.TestId,
                        QuestionId = row.QuestionId,
                        Order = row.Order
                    });
                }
                else
                {
                    existing.TestId = row.TestId;
                    existing.QuestionId = row.QuestionId;
                    existing.Order = row.Order;
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "testVragen", row.Id.ToString(), row.UpdatedAtUtc);
            }

            db.SaveChanges();
        }
    }
}
