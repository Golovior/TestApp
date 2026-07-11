using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class Opdrachten : IOpdrachtenStore
    {
        public Opdrachten() { }

        public List<string> GetOpdrachten()
        {
            using AppDbContext db = new();
            return db.Opdrachten
                .AsNoTracking()
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToList();
        }

        public bool OpdrachtAlreadyExists(string name)
        {
            using AppDbContext db = new();
            return db.Opdrachten.Any(x => x.Name.ToLower() == name.ToLower());
        }

        public void AddOpdracht(string name)
        {
            using AppDbContext db = new();
            Guid id = Guid.NewGuid();
            db.Opdrachten.Add(new Opdracht { Id = id, Name = name });
            RecordSyncHelper.TouchRecordTimestamp(db, "opdrachten", id.ToString());
            db.SaveChanges();
        }

        public void DeleteOpdracht(string name)
        {
            using AppDbContext db = new();
            Opdracht? entity = db.Opdrachten.SingleOrDefault(x => x.Name == name);
            if (entity == null)
                return;

            SoftDeleteHelper.Opdracht(db, entity.Id, RecordSyncHelper.GetCurrentUnixTimeSeconds());
            db.SaveChanges();
        }

        public void SaveOpdrachten()
        {
            // Persisted directly on each mutating operation.
        }

        public List<OpdrachtSyncDto> GetForSync()
        {
            using AppDbContext db = new();
            List<OpdrachtSyncDto> rows = db.Opdrachten.IgnoreQueryFilters().AsNoTracking().Select(x => new OpdrachtSyncDto
            {
                Id = x.Id,
                Name = x.Name,
                Deleted = x.Deleted
            }).ToList();

            foreach (OpdrachtSyncDto row in rows)
                row.UpdatedAtUtc = RecordSyncHelper.GetRecordTimestamp(db, "opdrachten", row.Id.ToString()) ?? 0;

            return rows;
        }

        public void ApplyFromSync(List<OpdrachtSyncDto> rows)
        {
            using AppDbContext db = new();

            foreach (OpdrachtSyncDto row in rows)
            {
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "opdrachten", row.Id.ToString(), row.UpdatedAtUtc))
                    continue;

                Opdracht? existing = db.Opdrachten.IgnoreQueryFilters().SingleOrDefault(x => x.Id == row.Id);
                if (existing == null)
                    db.Opdrachten.Add(new Opdracht { Id = row.Id, Name = row.Name, Deleted = row.Deleted });
                else
                {
                    existing.Name = row.Name;
                    existing.Deleted = row.Deleted;
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "opdrachten", row.Id.ToString(), row.UpdatedAtUtc);
            }

            db.SaveChanges();
        }
    }
}
