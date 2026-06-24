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

        public bool QuestionAlreadyExists(string test, string opdracht, string question)
        {
            using AppDbContext db = new();
            return db.TestQuestions.Any(x =>
                x.TestName == test
                && x.Opdracht == opdracht
                && x.QuestionText == question);
        }

        public List<List<string>> GetAllTestVragen()
        {
            using AppDbContext db = new();
            return db.TestQuestions
                .AsNoTracking()
                .Select(x => new List<string>
                {
                    x.TestName,
                    x.Opdracht,
                    x.QuestionText,
                    x.Order
                })
                .ToList();
        }

        public void AddTestVraag(string test, string opdracht, string question, string order)
        {
            using AppDbContext db = new();
            db.TestQuestions.Add(new TestQuestion
            {
                TestName = test,
                Opdracht = opdracht,
                QuestionText = question,
                Order = order
            });
            db.SaveChanges();
        }

        public void RemoveTestVragen(List<string> vraag)
        {
            if (vraag.Count < 4)
                return;

            using AppDbContext db = new();
            TestQuestion? current = db.TestQuestions.FirstOrDefault(x =>
                x.TestName == vraag[0]
                && x.Opdracht == vraag[1]
                && x.QuestionText == vraag[2]
                && x.Order == vraag[3]);

            if (current != null)
            {
                db.TestQuestions.Remove(current);
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

        public void UpdateFromApi(string data)
        {
            List<List<string>> rows = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(data) ?? new();

            using AppDbContext db = new();
            using var transaction = db.Database.BeginTransaction();
            db.TestQuestions.RemoveRange(db.TestQuestions);

            foreach (List<string> row in rows)
            {
                if (row.Count < 4)
                    continue;

                db.TestQuestions.Add(new TestQuestion
                {
                    TestName = row[0],
                    Opdracht = row[1],
                    QuestionText = row[2],
                    Order = row[3]
                });
            }

            db.SaveChanges();
            transaction.Commit();
        }

    }
}

