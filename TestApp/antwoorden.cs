using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class Antwoorden : IAnswerStore
    {
        public Antwoorden() { }

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

        private static Guid EnsureQuestionId(AppDbContext db, string opdracht, string vraag)
        {
            Guid opdrachtId = EnsureOpdrachtId(db, opdracht);
            Question? question = db.Questions.SingleOrDefault(x => x.OpdrachtId == opdrachtId && x.Text == vraag);
            if (question != null)
                return question.Id;

            question = new Question
            {
                Id = Guid.NewGuid(),
                OpdrachtId = opdrachtId,
                Text = vraag,
                Alphabetical = vraag
            };
            db.Questions.Add(question);
            db.SaveChanges();
            return question.Id;
        }

        public List<List<string>> GetAntwoorden()
        {
            using AppDbContext db = new();
            return db.Answers
                .AsNoTracking()
                .Include(x => x.Question)
                .ThenInclude(x => x.Opdracht)
                .Select(x => new List<string>
                {
                    x.Question.Opdracht.Name,
                    x.Question.Text,
                    x.Name,
                    x.IsCorrect ? "1" : "0",
                    x.ConnectedPlayersJson
                })
                .ToList();
        }

        public bool AntwoordAlreadyExists(string opdracht, string vraag, string name)
        {
            using AppDbContext db = new();
            Guid? questionId = db.Questions
                .Where(x => x.Opdracht.Name == opdracht && x.Text == vraag)
                .Select(x => (Guid?)x.Id)
                .SingleOrDefault();

            if (!questionId.HasValue)
                return false;

            return db.Answers.Any(x =>
                x.QuestionId == questionId.Value
                && x.Name.ToLower() == name.ToLower());
        }

        public void AddAntwoord(string opdracht, string vraag, string name, string correct = "0")
        {
            using AppDbContext db = new();
            Guid questionId = EnsureQuestionId(db, opdracht, vraag);
            Guid id = Guid.NewGuid();
            db.Answers.Add(new Answer
            {
                Id = id,
                QuestionId = questionId,
                Name = name,
                IsCorrect = correct == "1",
                ConnectedPlayersJson = "[]"
            });
            RecordSyncHelper.TouchRecordTimestamp(db, "antwoorden", id.ToString());
            db.SaveChanges();
        }

        public void SetAsCorrectAntwoord(string opdracht, string vraag, string name)
        {
            using AppDbContext db = new();
            using var transaction = db.Database.BeginTransaction();

            Guid? questionId = db.Questions
                .Where(x => x.Opdracht.Name == opdracht && x.Text == vraag)
                .Select(x => (Guid?)x.Id)
                .SingleOrDefault();

            if (!questionId.HasValue)
                return;

            List<Answer> antwoordSets = db.Answers
                .Where(x => x.QuestionId == questionId.Value)
                .ToList();

            foreach (Answer antwoordSet in antwoordSets)
            {
                antwoordSet.IsCorrect = name == antwoordSet.Name;
                RecordSyncHelper.TouchRecordTimestamp(db, "antwoorden", antwoordSet.Id.ToString());
            }

            db.SaveChanges();
            transaction.Commit();
        }

        public void ConnectPlayersToAnswer(string opdracht, string vraag, string antwoord, List<string> spelers)
        {
            string connectedPlayers = JsonSerializer.Serialize(spelers);

            using AppDbContext db = new();
            Guid? questionId = db.Questions
                .Where(x => x.Opdracht.Name == opdracht && x.Text == vraag)
                .Select(x => (Guid?)x.Id)
                .SingleOrDefault();

            if (!questionId.HasValue)
                return;

            Answer? antwoordSet = db.Answers.SingleOrDefault(x =>
                x.QuestionId == questionId.Value
                && x.Name == antwoord);

            if (antwoordSet != null)
            {
                antwoordSet.ConnectedPlayersJson = connectedPlayers;
                RecordSyncHelper.TouchRecordTimestamp(db, "antwoorden", antwoordSet.Id.ToString());
            }

            db.SaveChanges();
        }

        public void SaveAntwoorden()
        {
            // Persisted directly on each mutating operation.
        }

        public List<AnswerSyncDto> GetForSync()
        {
            using AppDbContext db = new();
            List<AnswerSyncDto> rows = db.Answers.AsNoTracking().Select(x => new AnswerSyncDto
            {
                Id = x.Id,
                QuestionId = x.QuestionId,
                Name = x.Name,
                IsCorrect = x.IsCorrect,
                ConnectedPlayersJson = x.ConnectedPlayersJson
            }).ToList();

            foreach (AnswerSyncDto row in rows)
                row.UpdatedAtUtc = RecordSyncHelper.GetRecordTimestamp(db, "antwoorden", row.Id.ToString()) ?? 0;

            return rows;
        }

        public void ApplyFromSync(List<AnswerSyncDto> rows)
        {
            using AppDbContext db = new();

            foreach (AnswerSyncDto row in rows)
            {
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "antwoorden", row.Id.ToString(), row.UpdatedAtUtc))
                    continue;

                Answer? existing = db.Answers.Find(row.Id);
                if (existing == null)
                {
                    db.Answers.Add(new Answer
                    {
                        Id = row.Id,
                        QuestionId = row.QuestionId,
                        Name = row.Name,
                        IsCorrect = row.IsCorrect,
                        ConnectedPlayersJson = row.ConnectedPlayersJson
                    });
                }
                else
                {
                    existing.QuestionId = row.QuestionId;
                    existing.Name = row.Name;
                    existing.IsCorrect = row.IsCorrect;
                    existing.ConnectedPlayersJson = row.ConnectedPlayersJson;
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "antwoorden", row.Id.ToString(), row.UpdatedAtUtc);
            }

            db.SaveChanges();
        }
    }
}
