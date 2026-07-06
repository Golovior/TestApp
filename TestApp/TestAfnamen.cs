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
    }
}
