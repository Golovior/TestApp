using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            db.Opdrachten.Add(new Opdracht { Id = Guid.NewGuid(), Name = name });
            RecordSyncHelper.TouchRecordTimestamp(db, "opdrachten", new[] { name });
            db.SaveChanges();
        }

        public string GetOpdrachtenInfo()
        {
            using AppDbContext db = new();
            List<string> appOpdrachten = db.Opdrachten
                .AsNoTracking()
                .Select(x => x.Name)
                .ToList();
            return JsonSerializer.Serialize(appOpdrachten);
        }

        public void SaveOpdrachten()
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
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "opdrachten", keyParts, remoteTimestamp))
                    continue;

                bool exists = db.Opdrachten.Any(x => x.Name == value);
                if (!exists)
                    db.Opdrachten.Add(new Opdracht { Id = Guid.NewGuid(), Name = value });

                RecordSyncHelper.TouchRecordTimestamp(db, "opdrachten", keyParts, remoteTimestamp);
            }

            db.SaveChanges();
            transaction.Commit();
        }

    }
}

