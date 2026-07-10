using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class TestAntwoorden : ITestAntwoordenStore
    {
        public TestAntwoorden() { }

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

            Guid id = Guid.NewGuid();
            db.TestAnswers.Add(new TestAnswer
            {
                Id = id,
                TestAfnameId = testAfnameId,
                TestQuestionId = testQuestion.Id,
                AnswerId = answerId.Value
            });

            RecordSyncHelper.TouchRecordTimestamp(db, "testAntwoorden", id.ToString());
            db.SaveChanges();
            return true;
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

        public void SaveTestAntwoorden()
        {
            // Persisted directly on each mutating operation.
        }

        public List<TestAnswerSyncDto> GetForSync()
        {
            using AppDbContext db = new();
            List<TestAnswerSyncDto> rows = db.TestAnswers.AsNoTracking().Select(x => new TestAnswerSyncDto
            {
                Id = x.Id,
                TestAfnameId = x.TestAfnameId,
                TestQuestionId = x.TestQuestionId,
                AnswerId = x.AnswerId
            }).ToList();

            foreach (TestAnswerSyncDto row in rows)
                row.UpdatedAtUtc = RecordSyncHelper.GetRecordTimestamp(db, "testAntwoorden", row.Id.ToString()) ?? 0;

            return rows;
        }

        public void ApplyFromSync(List<TestAnswerSyncDto> rows)
        {
            using AppDbContext db = new();

            foreach (TestAnswerSyncDto row in rows)
            {
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "testAntwoorden", row.Id.ToString(), row.UpdatedAtUtc))
                    continue;

                TestAnswer? existing = db.TestAnswers.Find(row.Id);
                if (existing == null)
                {
                    db.TestAnswers.Add(new TestAnswer
                    {
                        Id = row.Id,
                        TestAfnameId = row.TestAfnameId,
                        TestQuestionId = row.TestQuestionId,
                        AnswerId = row.AnswerId
                    });
                }
                else
                {
                    existing.TestAfnameId = row.TestAfnameId;
                    existing.TestQuestionId = row.TestQuestionId;
                    existing.AnswerId = row.AnswerId;
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "testAntwoorden", row.Id.ToString(), row.UpdatedAtUtc);
            }

            db.SaveChanges();
        }
    }
}
