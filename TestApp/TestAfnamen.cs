using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class TestAfnamen : ITestAfnameStore
    {
        public TestAfnamen() { }

        private static Guid EnsureTestId(AppDbContext db, string testName)
        {
            Test? test = db.Tests
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .FirstOrDefault(x => x.Name == testName);
            if (test != null)
                return test.Id;

            test = new Test { Id = Guid.NewGuid(), Name = testName };
            db.Tests.Add(test);
            db.SaveChanges();
            return test.Id;
        }

        private static Guid EnsurePlayerId(AppDbContext db, string speler)
        {
            Player? player = db.Players
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .FirstOrDefault(x => x.Name == speler);
            if (player != null)
                return player.Id;

            player = new Player
            {
                Id = Guid.NewGuid(),
                Name = speler
            };
            db.Players.Add(player);
            db.SaveChanges();
            return player.Id;
        }

        public Guid StartAfname(string test, string speler)
        {
            using AppDbContext db = new();
            Guid testId = EnsureTestId(db, test);
            Guid spelerId = EnsurePlayerId(db, speler);

            TestAfname afname = new()
            {
                Id = Guid.NewGuid(),
                TestId = testId,
                SpelerId = spelerId,
                Starttijd = DateTime.Now
            };

            db.TestAfnamen.Add(afname);
            RecordSyncHelper.TouchRecordTimestamp(db, "testAfnamen", afname.Id.ToString());
            db.SaveChanges();
            return afname.Id;
        }

        public void EindeAfname(Guid id)
        {
            using AppDbContext db = new();
            TestAfname? afname = db.TestAfnamen.SingleOrDefault(x => x.Id == id);
            if (afname == null)
                return;

            afname.Eindtijd = DateTime.Now;
            RecordSyncHelper.TouchRecordTimestamp(db, "testAfnamen", afname.Id.ToString());
            db.SaveChanges();
        }

        public List<List<string>> GetAllTestAfnamen()
        {
            using AppDbContext db = new();
            return db.TestAfnamen
                .AsNoTracking()
                .Include(x => x.Test)
                .Include(x => x.Speler)
                .Select(x => new List<string>
                {
                    x.Test.Name,
                    x.Speler.Name,
                    x.Starttijd.ToString("o", CultureInfo.InvariantCulture),
                    x.Eindtijd.HasValue ? x.Eindtijd.Value.ToString("o", CultureInfo.InvariantCulture) : string.Empty,
                    x.Jokers.HasValue ? x.Jokers.Value.ToString(CultureInfo.InvariantCulture) : string.Empty
                })
                .ToList();
        }

        public List<TestAfnameSyncDto> GetForSync()
        {
            using AppDbContext db = new();
            List<TestAfnameSyncDto> rows = db.TestAfnamen.AsNoTracking().Select(x => new TestAfnameSyncDto
            {
                Id = x.Id,
                TestId = x.TestId,
                SpelerId = x.SpelerId,
                Starttijd = x.Starttijd,
                Eindtijd = x.Eindtijd,
                Jokers = x.Jokers
            }).ToList();

            foreach (TestAfnameSyncDto row in rows)
                row.UpdatedAtUtc = RecordSyncHelper.GetRecordTimestamp(db, "testAfnamen", row.Id.ToString()) ?? 0;

            return rows;
        }

        public void ApplyFromSync(List<TestAfnameSyncDto> rows)
        {
            using AppDbContext db = new();

            foreach (TestAfnameSyncDto row in rows)
            {
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "testAfnamen", row.Id.ToString(), row.UpdatedAtUtc))
                    continue;

                TestAfname? existing = db.TestAfnamen.Find(row.Id);
                if (existing == null)
                {
                    db.TestAfnamen.Add(new TestAfname
                    {
                        Id = row.Id,
                        TestId = row.TestId,
                        SpelerId = row.SpelerId,
                        Starttijd = row.Starttijd,
                        Eindtijd = row.Eindtijd,
                        Jokers = row.Jokers
                    });
                }
                else
                {
                    existing.TestId = row.TestId;
                    existing.SpelerId = row.SpelerId;
                    existing.Starttijd = row.Starttijd;
                    existing.Eindtijd = row.Eindtijd;
                    existing.Jokers = row.Jokers;
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "testAfnamen", row.Id.ToString(), row.UpdatedAtUtc);
            }

            db.SaveChanges();
        }
    }
}
