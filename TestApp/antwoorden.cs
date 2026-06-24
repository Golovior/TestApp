using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            db.Answers.Add(new Answer
            {
                Id = Guid.NewGuid(),
                QuestionId = questionId,
                Name = name,
                IsCorrect = correct == "1",
                ConnectedPlayersJson = "[]"
            });
            RecordSyncHelper.TouchRecordTimestamp(db, "antwoorden", new[] { opdracht, vraag, name });
            db.SaveChanges();
        }

        public string GetAntwoordenInfo()
        {
            List<List<string>> appAntwoorden = GetAntwoorden();
            return JsonSerializer.Serialize(appAntwoorden);
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
                RecordSyncHelper.TouchRecordTimestamp(db, "antwoorden", new[] { opdracht, vraag, antwoordSet.Name });
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
                RecordSyncHelper.TouchRecordTimestamp(db, "antwoorden", new[] { opdracht, vraag, antwoord });
            }

            db.SaveChanges();
        }

        public void SaveAntwoorden()
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

                string[] keyParts = { row[0], row[1], row[2] };
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "antwoorden", keyParts, remoteTimestamp))
                    continue;

                Guid questionId = EnsureQuestionId(db, row[0], row[1]);

                Answer? current = db.Answers.SingleOrDefault(x => x.QuestionId == questionId && x.Name == row[2]);
                if (current == null)
                {
                    db.Answers.Add(new Answer
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = questionId,
                        Name = row[2],
                        IsCorrect = row[3] == "1",
                        ConnectedPlayersJson = row[4]
                    });
                }
                else
                {
                    current.IsCorrect = row[3] == "1";
                    current.ConnectedPlayersJson = row[4];
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "antwoorden", keyParts, remoteTimestamp);
            }

            db.SaveChanges();
            transaction.Commit();
        }
    }
}

