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
            db.Tests.Add(new Test { Id = Guid.NewGuid(), Name = test });
            RecordSyncHelper.TouchRecordTimestamp(db, "tests", new[] { test });
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

        public void UpdateFromApi(string data, long? remoteTimestamp = null)
        {
            List<string> values = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(data) ?? new();

            using AppDbContext db = new();
            using var transaction = db.Database.BeginTransaction();
            foreach (string value in values)
            {
                string[] keyParts = { value };
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "tests", keyParts, remoteTimestamp))
                    continue;

                bool exists = db.Tests.Any(x => x.Name == value);
                if (!exists)
                    db.Tests.Add(new Test { Id = Guid.NewGuid(), Name = value });

                RecordSyncHelper.TouchRecordTimestamp(db, "tests", keyParts, remoteTimestamp);
            }

            db.SaveChanges();
            transaction.Commit();
        }
    }
}
