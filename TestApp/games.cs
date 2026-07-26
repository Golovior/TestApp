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

        public List<string> GetAllGames()
        {
            using AppDbContext db = new();
            return db.Games.AsNoTracking().Select(x => x.Name).OrderBy(x => x).ToList();
        }

        public void AddGame(string name) {
            using AppDbContext db = new();
            Guid id = Guid.NewGuid();
            db.Games.Add(new Game { Id = id, Name = name });
            RecordSyncHelper.TouchRecordTimestamp(db, "games", id.ToString());
            db.SaveChanges();
        }

        public void DeleteGame(string name)
        {
            using AppDbContext db = new();
            Game? entity = db.Games.SingleOrDefault(x => x.Name == name);
            if (entity == null)
                return;

            SoftDeleteHelper.Game(db, entity.Id, RecordSyncHelper.GetCurrentUnixTimeSeconds());
            db.SaveChanges();
        }

        public void SaveGames()
        {
            // Persisted directly on each mutating operation.
        }

        public List<GameSyncDto> GetForSync()
        {
            using AppDbContext db = new();
            List<GameSyncDto> rows = db.Games.IgnoreQueryFilters().AsNoTracking().Select(x => new GameSyncDto
            {
                Id = x.Id,
                Name = x.Name,
                Deleted = x.Deleted
            }).ToList();

            foreach (GameSyncDto row in rows)
                row.UpdatedAtUtc = RecordSyncHelper.GetRecordTimestamp(db, "games", row.Id.ToString()) ?? 0;

            return rows;
        }

        public void ApplyFromSync(List<GameSyncDto> rows)
        {
            using AppDbContext db = new();

            foreach (GameSyncDto row in rows)
            {
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "games", row.Id.ToString(), row.UpdatedAtUtc))
                    continue;

                Game? existing = db.Games.IgnoreQueryFilters().SingleOrDefault(x => x.Id == row.Id);
                if (existing == null)
                    db.Games.Add(new Game { Id = row.Id, Name = row.Name, Deleted = row.Deleted });
                else
                {
                    existing.Name = row.Name;
                    existing.Deleted = row.Deleted;
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "games", row.Id.ToString(), row.UpdatedAtUtc);
            }

            db.SaveChanges();
        }
    }
}
