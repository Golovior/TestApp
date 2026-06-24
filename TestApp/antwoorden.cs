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

        public List<List<string>> GetAntwoorden()
        {
            using AppDbContext db = new();
            return db.Answers
                .AsNoTracking()
                .Select(x => new List<string>
                {
                    x.Opdracht,
                    x.Vraag,
                    x.Name,
                    x.IsCorrect ? "1" : "0",
                    x.ConnectedPlayersJson
                })
                .ToList();
        }

        public bool AntwoordAlreadyExists(string opdracht, string vraag, string name)
        {
            using AppDbContext db = new();
            return db.Answers.Any(x =>
                x.Opdracht == opdracht
                && x.Vraag == vraag
                && x.Name.ToLower() == name.ToLower());
        }

        public void AddAntwoord(string opdracht, string vraag, string name, string correct = "0")
        {
            using AppDbContext db = new();
            db.Answers.Add(new Answer
            {
                Opdracht = opdracht,
                Vraag = vraag,
                Name = name,
                IsCorrect = correct == "1",
                ConnectedPlayersJson = "[]"
            });
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

            List<Answer> antwoordSets = db.Answers
                .Where(x => x.Opdracht == opdracht && x.Vraag == vraag)
                .ToList();

            foreach (Answer antwoordSet in antwoordSets)
            {
                antwoordSet.IsCorrect = name == antwoordSet.Name;
            }

            db.SaveChanges();
            transaction.Commit();
        }

        public void ConnectPlayersToAnswer(string opdracht, string vraag, string antwoord, List<string> spelers)
        {
            string connectedPlayers = JsonSerializer.Serialize(spelers);

            using AppDbContext db = new();
            Answer? antwoordSet = db.Answers.SingleOrDefault(x =>
                x.Opdracht == opdracht
                && x.Vraag == vraag
                && x.Name == antwoord);

            if (antwoordSet != null)
                antwoordSet.ConnectedPlayersJson = connectedPlayers;

            db.SaveChanges();
        }

        public void SaveAntwoorden()
        {
            // Persisted directly on each mutating operation.
        }

        public void UpdateFromApi(string data)
        {
            List<List<string>> rows = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(data) ?? new();

            using AppDbContext db = new();
            using var transaction = db.Database.BeginTransaction();
            db.Answers.RemoveRange(db.Answers);

            foreach (List<string> row in rows)
            {
                if (row.Count < 5)
                    continue;

                db.Answers.Add(new Answer
                {
                    Opdracht = row[0],
                    Vraag = row[1],
                    Name = row[2],
                    IsCorrect = row[3] == "1",
                    ConnectedPlayersJson = row[4]
                });
            }

            db.SaveChanges();
            transaction.Commit();
        }
    }
}

