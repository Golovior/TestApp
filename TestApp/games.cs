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
            db.Games.Add(new Game { Name = name });
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

        public void UpdateFromApi(string data)
        {
            List<string> values = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(data) ?? new();

            using AppDbContext db = new();
            using var transaction = db.Database.BeginTransaction();
            db.Games.RemoveRange(db.Games);
            foreach (string value in values)
                db.Games.Add(new Game { Name = value });

            db.SaveChanges();
            transaction.Commit();
        }

    }
}

