using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class Questions : IQuestionStore
    {
        public Questions() { }

        private static Guid EnsureOpdrachtId(AppDbContext db, string opdracht)
        {
            Opdracht? existing = db.Opdrachten.SingleOrDefault(x => x.Name == opdracht);
            if (existing != null)
                return existing.Id;

            existing = new Opdracht { Id = Guid.NewGuid(), Name = opdracht };
            db.Opdrachten.Add(existing);
            db.SaveChanges();
            return existing.Id;
        }

        public bool QuestionAlreadyExists(string opdracht, string question) {
            using AppDbContext db = new();
            Guid? opdrachtId = db.Opdrachten.Where(x => x.Name == opdracht).Select(x => (Guid?)x.Id).SingleOrDefault();
            if (!opdrachtId.HasValue)
                return false;

            return db.Questions.Any(q => q.OpdrachtId == opdrachtId.Value && q.Text.ToLower() == question.ToLower());
        }

        public List<List<string>> GetAllQuestions()
        {
            using AppDbContext db = new();
            return db.Questions
                .AsNoTracking()
                .Include(x => x.Opdracht)
                .OrderBy(x => x.Text)
                .Select(x => new List<string>
                {
                    x.Opdracht.Name,
                    x.Text,
                    x.Alphabetical
                })
                .ToList();
        }

        public void AddQuestion(string opdracht, string question, string alphabetical) {
            using AppDbContext db = new();
            Guid opdrachtId = EnsureOpdrachtId(db, opdracht);
            Guid id = Guid.NewGuid();
            db.Questions.Add(new Question
            {
                Id = id,
                OpdrachtId = opdrachtId,
                Text = question,
                Alphabetical = alphabetical
            });
            RecordSyncHelper.TouchRecordTimestamp(db, "questions", id.ToString());
            db.SaveChanges();
        }

        public void SaveQuestions()
        {
            // Persisted directly on each mutating operation.
        }

        public List<QuestionSyncDto> GetForSync()
        {
            using AppDbContext db = new();
            List<QuestionSyncDto> rows = db.Questions.AsNoTracking().Select(x => new QuestionSyncDto
            {
                Id = x.Id,
                OpdrachtId = x.OpdrachtId,
                Text = x.Text,
                Alphabetical = x.Alphabetical
            }).ToList();

            foreach (QuestionSyncDto row in rows)
                row.UpdatedAtUtc = RecordSyncHelper.GetRecordTimestamp(db, "questions", row.Id.ToString()) ?? 0;

            return rows;
        }

        public void ApplyFromSync(List<QuestionSyncDto> rows)
        {
            using AppDbContext db = new();

            foreach (QuestionSyncDto row in rows)
            {
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "questions", row.Id.ToString(), row.UpdatedAtUtc))
                    continue;

                Question? existing = db.Questions.Find(row.Id);
                if (existing == null)
                {
                    db.Questions.Add(new Question
                    {
                        Id = row.Id,
                        OpdrachtId = row.OpdrachtId,
                        Text = row.Text,
                        Alphabetical = row.Alphabetical
                    });
                }
                else
                {
                    existing.OpdrachtId = row.OpdrachtId;
                    existing.Text = row.Text;
                    existing.Alphabetical = row.Alphabetical;
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "questions", row.Id.ToString(), row.UpdatedAtUtc);
            }

            db.SaveChanges();
        }
    }
}
