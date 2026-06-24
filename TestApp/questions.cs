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
            db.Questions.Add(new Question
            {
                OpdrachtId = opdrachtId,
                Text = question,
                Alphabetical = alphabetical
            });
            RecordSyncHelper.TouchRecordTimestamp(db, "questions", new[] { opdracht, question });
            db.SaveChanges();
        }

        public string GetQuestionInfo()
        {
            List<List<string>> appQuestions = GetAllQuestions();
            return JsonSerializer.Serialize(appQuestions);
        }

        public void SaveQuestions()
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
                if (row.Count < 3)
                    continue;

                string[] keyParts = { row[0], row[1] };
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "questions", keyParts, remoteTimestamp))
                    continue;

                Guid opdrachtId = EnsureOpdrachtId(db, row[0]);
                Question? current = db.Questions.SingleOrDefault(x => x.OpdrachtId == opdrachtId && x.Text == row[1]);
                if (current == null)
                {
                    db.Questions.Add(new Question
                    {
                        OpdrachtId = opdrachtId,
                        Text = row[1],
                        Alphabetical = row[2]
                    });
                }
                else
                {
                    current.Alphabetical = row[2];
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "questions", keyParts, remoteTimestamp);
            }

            db.SaveChanges();
            transaction.Commit();
        }

    }
}

