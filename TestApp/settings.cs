using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class Settings : ISettingsStore
    {
        readonly List<string> keys = new();

        public Settings() {
            keys.Add("ActiveGame");
        }

        public List<string> GetKeys()
        {
            return keys;
        }

        public void UpdateSetting(string key, string value) {
            if (!keys.Contains(key))
                return;

            using AppDbContext db = new();
            SettingEntry? current = db.Settings.SingleOrDefault(x => x.Key == key);

            if (current == null)
            {
                db.Settings.Add(new SettingEntry
                {
                    Key = key,
                    Value = value
                });
            }
            else
            {
                current.Value = value;
            }

            RecordSyncHelper.TouchRecordTimestamp(db, "settings", new[] { key });

            db.SaveChanges();
        }

        public string GetSettingsInfo()
        {
            using AppDbContext db = new();
            List<KeyValuePair<string, string>> pairs = db.Settings
                .AsNoTracking()
                .Select(x => new KeyValuePair<string, string>(x.Key, x.Value))
                .ToList();
            return JsonSerializer.Serialize(pairs);
        }

        public void UpdateFromApi(string data, long? remoteTimestamp = null)
        {
            List<KeyValuePair<string, string>> pairs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(data) ?? new();

            using AppDbContext db = new();
            using var transaction = db.Database.BeginTransaction();
            foreach (KeyValuePair<string, string> pair in pairs)
            {
                string[] keyParts = { pair.Key };
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "settings", keyParts, remoteTimestamp))
                    continue;

                SettingEntry? current = db.Settings.SingleOrDefault(x => x.Key == pair.Key);
                if (current == null)
                {
                    db.Settings.Add(new SettingEntry
                    {
                        Key = pair.Key,
                        Value = pair.Value
                    });
                }
                else
                {
                    current.Value = pair.Value;
                }

                RecordSyncHelper.TouchRecordTimestamp(db, "settings", keyParts, remoteTimestamp);
            }

            db.SaveChanges();
            transaction.Commit();
        }

    }
}
