using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class Tests : ITestsStore
    {
        public Tests() { }

        public bool TestAlreadyExists(string test)
        {
            using AppDbContext db = new();
            return db.Tests.Any(x => x.Name.ToLower() == test.ToLower());
        }

        public List<string> GetAllTests()
        {
            using AppDbContext db = new();
            return db.Tests
                .AsNoTracking()
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToList();
        }

        public void AddTest(string test)
        {
            using AppDbContext db = new();
            db.Tests.Add(new Test { Name = test });
            db.SaveChanges();
        }

        public string GetTestInfo()
        {
            using AppDbContext db = new();
            List<string> appTests = db.Tests
                .AsNoTracking()
                .Select(x => x.Name)
                .ToList();
            return JsonSerializer.Serialize(appTests);
        }

        public void SaveTests()
        {
            // Persisted directly on each mutating operation.
        }

        public void UpdateFromApi(string data)
        {
            List<string> values = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(data) ?? new();

            using AppDbContext db = new();
            using var transaction = db.Database.BeginTransaction();
            db.Tests.RemoveRange(db.Tests);
            foreach (string value in values)
                db.Tests.Add(new Test { Name = value });

            db.SaveChanges();
            transaction.Commit();
        }
    }
}
