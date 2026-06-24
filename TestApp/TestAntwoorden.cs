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

        public bool AntwoordAlreadyExists(string test, string speler, string opdracht, string question, string antwoord)
        {
            using AppDbContext db = new();
            return db.TestAnswers.Any(x =>
                x.TestName == test
                && x.Speler == speler
                && x.Opdracht == opdracht
                && x.QuestionText == question
                && x.AnswerText == antwoord);
        }

        public List<List<string>> GetAllTestAntwoorden()
        {
            using AppDbContext db = new();
            return db.TestAnswers
                .AsNoTracking()
                .Select(x => new List<string>
                {
                    x.TestName,
                    x.Speler,
                    x.Opdracht,
                    x.QuestionText,
                    x.AnswerText
                })
                .ToList();
        }

        public void AddTestAntwoord(string test, string speler, string opdracht, string question, string antwoord)
        {
            using AppDbContext db = new();
            db.TestAnswers.Add(new TestAnswer
            {
                TestName = test,
                Speler = speler,
                Opdracht = opdracht,
                QuestionText = question,
                AnswerText = antwoord
            });
            db.SaveChanges();
        }

        public void RemoveTestAntwoord(List<string> antwoord)
        {
            if (antwoord.Count < 5)
                return;

            using AppDbContext db = new();
            TestAnswer? current = db.TestAnswers.FirstOrDefault(x =>
                x.TestName == antwoord[0]
                && x.Speler == antwoord[1]
                && x.Opdracht == antwoord[2]
                && x.QuestionText == antwoord[3]
                && x.AnswerText == antwoord[4]);

            if (current != null)
            {
                db.TestAnswers.Remove(current);
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

        public void UpdateFromApi(string data)
        {
            List<List<string>> rows = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(data) ?? new();

            using AppDbContext db = new();
            using var transaction = db.Database.BeginTransaction();
            db.TestAnswers.RemoveRange(db.TestAnswers);

            foreach (List<string> row in rows)
            {
                if (row.Count < 5)
                    continue;

                db.TestAnswers.Add(new TestAnswer
                {
                    TestName = row[0],
                    Speler = row[1],
                    Opdracht = row[2],
                    QuestionText = row[3],
                    AnswerText = row[4]
                });
            }

            db.SaveChanges();
            transaction.Commit();
        }

    }
}

