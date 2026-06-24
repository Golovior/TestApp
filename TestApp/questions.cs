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

        public bool QuestionAlreadyExists(string opdracht, string question) {
            using AppDbContext db = new();
            return db.Questions.Any(q => q.Opdracht == opdracht && q.Text.ToLower() == question.ToLower());
        }

        public List<List<string>> GetAllQuestions()
        {
            using AppDbContext db = new();
            return db.Questions
                .AsNoTracking()
                .OrderBy(x => x.Text)
                .Select(x => new List<string>
                {
                    x.Opdracht,
                    x.Text,
                    x.Alphabetical
                })
                .ToList();
        }

        public void AddQuestion(string opdracht, string question, string alphabetical) {
            using AppDbContext db = new();
            db.Questions.Add(new Question
            {
                Opdracht = opdracht,
                Text = question,
                Alphabetical = alphabetical
            });
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

        public void UpdateFromApi(string data)
        {
            List<List<string>> rows = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(data) ?? new();

            using AppDbContext db = new();
            using var transaction = db.Database.BeginTransaction();
            db.Questions.RemoveRange(db.Questions);

            foreach (List<string> row in rows)
            {
                if (row.Count < 3)
                    continue;

                db.Questions.Add(new Question
                {
                    Opdracht = row[0],
                    Text = row[1],
                    Alphabetical = row[2]
                });
            }

            db.SaveChanges();
            transaction.Commit();
        }

    }
}

