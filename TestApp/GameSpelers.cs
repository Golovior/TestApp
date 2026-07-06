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
                db.GameSpelers.Add(new GameSpeler
                {
                    Id = Guid.NewGuid(),
                    GameId = gameId.Value,
                    SpelerId = spelerId,
                    Status = "1"
                });
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
                db.SaveChanges();
            }
        }
    }
}
