using System;
using System.Collections.Generic;
using System.Linq;
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
            Guid id = Guid.NewGuid();
            db.Tests.Add(new Test { Id = id, Name = test });
            RecordSyncHelper.TouchRecordTimestamp(db, "tests", id.ToString());
            db.SaveChanges();
        }

        public void SetGameForTest(string test, string? game)
        {
            using AppDbContext db = new();
            Test? entity = db.Tests.FirstOrDefault(x => x.Name == test);
            if (entity == null)
                return;

            entity.GameId = string.IsNullOrEmpty(game)
                ? null
                : db.Games.Where(x => x.Name == game).Select(x => (Guid?)x.Id).FirstOrDefault();

            RecordSyncHelper.TouchRecordTimestamp(db, "tests", entity.Id.ToString());

            db.SaveChanges();
        }

        public string? GetGameForTest(string test)
        {
            using AppDbContext db = new();
            return db.Tests
                .Where(x => x.Name == test)
                .Select(x => x.Game != null ? x.Game.Name : null)
                .FirstOrDefault();
        }

        public void DeleteTest(string test)
        {
            using AppDbContext db = new();
            Test? entity = db.Tests.SingleOrDefault(x => x.Name == test);
            if (entity == null)
                return;

            SoftDeleteHelper.Test(db, entity.Id, RecordSyncHelper.GetCurrentUnixTimeSeconds());
            db.SaveChanges();
        }

        public void SaveTests()
        {
            // Persisted directly on each mutating operation.
        }

        public List<TestSyncDto> GetForSync()
        {
            using AppDbContext db = new();
            List<TestSyncDto> rows = db.Tests.IgnoreQueryFilters().AsNoTracking().Select(x => new TestSyncDto
            {
                Id = x.Id,
                Name = x.Name,
                GameId = x.GameId,
                Deleted = x.Deleted
            }).ToList();

            foreach (TestSyncDto row in rows)
                row.UpdatedAtUtc = RecordSyncHelper.GetRecordTimestamp(db, "tests", row.Id.ToString()) ?? 0;

            return rows;
        }

        public void ApplyFromSync(List<TestSyncDto> rows)
        {
            using AppDbContext db = new();

            foreach (TestSyncDto row in rows)
            {
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "tests", row.Id.ToString(), row.UpdatedAtUtc))
                    continue;

                Test? existing = db.Tests.IgnoreQueryFilters().SingleOrDefault(x => x.Id == row.Id);
                if (existing == null)
                    db.Tests.Add(new Test { Id = row.Id, Name = row.Name, GameId = row.GameId, Deleted = row.Deleted });
                else
                {
                    existing.Name = row.Name;
                    existing.GameId = row.GameId;
                    existing.Deleted = row.Deleted;
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "tests", row.Id.ToString(), row.UpdatedAtUtc);
            }

            db.SaveChanges();
        }
    }
}
