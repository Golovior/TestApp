using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class GameSpelers : IGameSpelersStore
    {
        public GameSpelers() { }

        private static Guid? FindGameId(AppDbContext db, string game)
        {
            return db.Games
                .Where(x => x.Name == game)
                .OrderBy(x => x.Id)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefault();
        }

        private static Guid EnsurePlayerId(AppDbContext db, string speler)
        {
            Player? player = db.Players
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .FirstOrDefault(x => x.Name == speler);
            if (player != null)
                return player.Id;

            player = new Player { Id = Guid.NewGuid(), Name = speler };
            db.Players.Add(player);
            db.SaveChanges();
            return player.Id;
        }

        public List<List<string>> GetSpelersForGame(string game)
        {
            using AppDbContext db = new();
            Guid? gameId = FindGameId(db, game);
            if (!gameId.HasValue)
                return new();

            return db.GameSpelers
                .AsNoTracking()
                .Include(x => x.Speler)
                .Where(x => x.GameId == gameId.Value)
                .OrderBy(x => x.Speler.Name)
                .Select(x => new List<string> { x.Speler.Name, x.Status })
                .ToList();
        }

        // Deliberately does not auto-create the Game: games only ever come from "Spel
        // aanmaken", so a typo'd game name should no-op rather than silently create one.
        public void AssignSpelerToGame(string game, string speler)
        {
            using AppDbContext db = new();
            Guid? gameId = FindGameId(db, game);
            if (!gameId.HasValue)
                return;

            Guid spelerId = EnsurePlayerId(db, speler);

            bool exists = db.GameSpelers.Any(x => x.GameId == gameId.Value && x.SpelerId == spelerId);
            if (!exists)
            {
                Guid id = Guid.NewGuid();
                db.GameSpelers.Add(new GameSpeler
                {
                    Id = id,
                    GameId = gameId.Value,
                    SpelerId = spelerId,
                    Status = "1"
                });
                RecordSyncHelper.TouchRecordTimestamp(db, "gameSpelers", id.ToString());
                db.SaveChanges();
            }
        }

        public void RemoveSpelerFromGame(string game, string speler)
        {
            using AppDbContext db = new();
            Guid? gameId = FindGameId(db, game);
            if (!gameId.HasValue)
                return;

            GameSpeler? row = db.GameSpelers.FirstOrDefault(x => x.GameId == gameId.Value && x.Speler.Name == speler);
            if (row != null)
            {
                RecordSyncHelper.TouchRecordTimestamp(db, "gameSpelers", row.Id.ToString());
                db.GameSpelers.Remove(row);
                db.SaveChanges();
            }
        }

        public void SetStatus(string game, string speler, string status)
        {
            using AppDbContext db = new();
            Guid? gameId = FindGameId(db, game);
            if (!gameId.HasValue)
                return;

            GameSpeler? row = db.GameSpelers.FirstOrDefault(x => x.GameId == gameId.Value && x.Speler.Name == speler);
            if (row != null)
            {
                row.Status = status;
                RecordSyncHelper.TouchRecordTimestamp(db, "gameSpelers", row.Id.ToString());
                db.SaveChanges();
            }
        }

        public List<GameSpelerSyncDto> GetForSync()
        {
            using AppDbContext db = new();
            List<GameSpelerSyncDto> rows = db.GameSpelers.AsNoTracking().Select(x => new GameSpelerSyncDto
            {
                Id = x.Id,
                GameId = x.GameId,
                SpelerId = x.SpelerId,
                Status = x.Status
            }).ToList();

            foreach (GameSpelerSyncDto row in rows)
                row.UpdatedAtUtc = RecordSyncHelper.GetRecordTimestamp(db, "gameSpelers", row.Id.ToString()) ?? 0;

            return rows;
        }

        public void ApplyFromSync(List<GameSpelerSyncDto> rows)
        {
            using AppDbContext db = new();

            foreach (GameSpelerSyncDto row in rows)
            {
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "gameSpelers", row.Id.ToString(), row.UpdatedAtUtc))
                    continue;

                GameSpeler? existing = db.GameSpelers.Find(row.Id);
                if (existing == null)
                {
                    db.GameSpelers.Add(new GameSpeler
                    {
                        Id = row.Id,
                        GameId = row.GameId,
                        SpelerId = row.SpelerId,
                        Status = row.Status
                    });
                }
                else
                {
                    existing.GameId = row.GameId;
                    existing.SpelerId = row.SpelerId;
                    existing.Status = row.Status;
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "gameSpelers", row.Id.ToString(), row.UpdatedAtUtc);
            }

            db.SaveChanges();
        }
    }
}
