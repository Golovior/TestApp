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

        public List<List<string>> GetSpelers()
        {
            using AppDbContext db = new();
            return db.Players
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new List<string>
                {
                    x.Name,
                    x.Status
                })
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
                Name = name,
                Status = "1"
            });
            RecordSyncHelper.TouchRecordTimestamp(db, "spelers", new[] { name });
            db.SaveChanges();
        }

        public void SavePlayerStatus(string name, string status)
        {
            using AppDbContext db = new();
            Player? speler = db.Players.SingleOrDefault(x => x.Name == name);
            if (speler != null)
            {
                speler.Status = status;
                RecordSyncHelper.TouchRecordTimestamp(db, "spelers", new[] { name });
                db.SaveChanges();
            }
        }

        public string GetSpelersInfo()
        {
            List<List<string>> appSpelers = GetSpelers();
            return JsonSerializer.Serialize(appSpelers);
        }

        public void SaveSpelers()
        {
            // Persisted directly on each mutating operation.
        }

        public void UpdateFromApi(string data, long? remoteTimestamp = null)
        {
            List<List<string>> rows = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(data) ?? new();

            using AppDbContext db = new();
            using var transaction = db.Database.BeginTransaction();

            foreach (List<string> row in rows)
            {
                if (row.Count < 2)
                    continue;

                string[] keyParts = { row[0] };
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "spelers", keyParts, remoteTimestamp))
                    continue;

                Player? current = db.Players.SingleOrDefault(x => x.Name == row[0]);
                if (current == null)
                {
                    db.Players.Add(new Player
                    {
                        Name = row[0],
                        Status = row[1]
                    });
                }
                else
                {
                    current.Status = row[1];
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "spelers", keyParts, remoteTimestamp);
            }

            db.SaveChanges();
            transaction.Commit();
        }

    }
}

