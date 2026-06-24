using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class Games : IGameStore
    {
        public Games() { }

        public bool GameAlreadyExists(string name) {
            using AppDbContext db = new();
            return db.Games.Any(x => x.Name == name);
        }

        public void AddGame(string name) {
            using AppDbContext db = new();
            db.Games.Add(new Game { Id = Guid.NewGuid(), Name = name });
            RecordSyncHelper.TouchRecordTimestamp(db, "games", new[] { name });
            db.SaveChanges();
        }

        public string GetGameInfo()
        {
            using AppDbContext db = new();
            List<string> appGames = db.Games
                .AsNoTracking()
                .Select(x => x.Name)
                .ToList();
            return JsonSerializer.Serialize(appGames);
        }

        public void SaveGames()
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
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "games", keyParts, remoteTimestamp))
                    continue;

                bool exists = db.Games.Any(x => x.Name == value);
                if (!exists)
                    db.Games.Add(new Game { Id = Guid.NewGuid(), Name = value });

                RecordSyncHelper.TouchRecordTimestamp(db, "games", keyParts, remoteTimestamp);
            }

            db.SaveChanges();
            transaction.Commit();
        }

    }
}

