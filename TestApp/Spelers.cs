using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            db.Players.Add(new Player
            {
                Name = name
            });
            RecordSyncHelper.TouchRecordTimestamp(db, "spelers", new[] { name });
            db.SaveChanges();
        }

        public string GetSpelersInfo()
        {
            List<string> appSpelers = GetSpelers();
            return JsonSerializer.Serialize(appSpelers);
        }

        public void SaveSpelers()
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
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "spelers", keyParts, remoteTimestamp))
                    continue;

                if (!db.Players.Any(x => x.Name == value))
                    db.Players.Add(new Player { Name = value });

                RecordSyncHelper.TouchRecordTimestamp(db, "spelers", keyParts, remoteTimestamp);
            }

            db.SaveChanges();
            transaction.Commit();
        }

    }
}

