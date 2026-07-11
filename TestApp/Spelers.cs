using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class Spelers : ISpelersStore
    {
        public Spelers() { }

        public List<string> GetSpelers()
        {
            using AppDbContext db = new();
            return db.Players
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => x.Name)
                .ToList();
        }

        public bool SpelerAlreadyExists(string name)
        {
            using AppDbContext db = new();
            return db.Players.Any(x => x.Name.ToLower() == name.ToLower());
        }

        public void AddSpeler(string name)
        {
            using AppDbContext db = new();
            Guid id = Guid.NewGuid();
            db.Players.Add(new Player
            {
                Id = id,
                Name = name
            });
            RecordSyncHelper.TouchRecordTimestamp(db, "spelers", id.ToString());
            db.SaveChanges();
        }

        public void DeleteSpeler(string name)
        {
            using AppDbContext db = new();
            Player? entity = db.Players.SingleOrDefault(x => x.Name == name);
            if (entity == null)
                return;

            SoftDeleteHelper.Player(db, entity.Id, RecordSyncHelper.GetCurrentUnixTimeSeconds());
            db.SaveChanges();
        }

        public void SaveSpelers()
        {
            // Persisted directly on each mutating operation.
        }

        public List<PlayerSyncDto> GetForSync()
        {
            using AppDbContext db = new();
            List<PlayerSyncDto> rows = db.Players.IgnoreQueryFilters().AsNoTracking().Select(x => new PlayerSyncDto
            {
                Id = x.Id,
                Name = x.Name,
                Deleted = x.Deleted
            }).ToList();

            foreach (PlayerSyncDto row in rows)
                row.UpdatedAtUtc = RecordSyncHelper.GetRecordTimestamp(db, "spelers", row.Id.ToString()) ?? 0;

            return rows;
        }

        public void ApplyFromSync(List<PlayerSyncDto> rows)
        {
            using AppDbContext db = new();

            foreach (PlayerSyncDto row in rows)
            {
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "spelers", row.Id.ToString(), row.UpdatedAtUtc))
                    continue;

                Player? existing = db.Players.IgnoreQueryFilters().SingleOrDefault(x => x.Id == row.Id);
                if (existing == null)
                    db.Players.Add(new Player { Id = row.Id, Name = row.Name, Deleted = row.Deleted });
                else
                {
                    existing.Name = row.Name;
                    existing.Deleted = row.Deleted;
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "spelers", row.Id.ToString(), row.UpdatedAtUtc);
            }

            db.SaveChanges();
        }
    }
}
