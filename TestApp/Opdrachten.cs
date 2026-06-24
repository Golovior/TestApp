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
            db.Opdrachten.Add(new Opdracht { Name = name });
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

        public void UpdateFromApi(string data)
        {
            List<string> values = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(data) ?? new();

            using AppDbContext db = new();
            using var transaction = db.Database.BeginTransaction();
            db.Opdrachten.RemoveRange(db.Opdrachten);
            foreach (string value in values)
                db.Opdrachten.Add(new Opdracht { Name = value });

            db.SaveChanges();
            transaction.Commit();
        }

    }
}

