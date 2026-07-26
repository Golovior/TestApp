using System;
using System.Collections.Generic;
using System.Linq;
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

            RecordSyncHelper.TouchRecordTimestamp(db, "settings", key);

            db.SaveChanges();
        }

        // Excludes the "SyncRecord:*" bookkeeping rows that RecordSyncHelper itself
        // writes into this same table - those are sync metadata, not real settings.
        public List<SettingSyncDto> GetForSync()
        {
            using AppDbContext db = new();
            List<SettingSyncDto> rows = db.Settings
                .AsNoTracking()
                .Where(x => !x.Key.StartsWith("SyncRecord:"))
                .Select(x => new SettingSyncDto { Key = x.Key, Value = x.Value })
                .ToList();

            foreach (SettingSyncDto row in rows)
                row.UpdatedAtUtc = RecordSyncHelper.GetRecordTimestamp(db, "settings", row.Key) ?? 0;

            return rows;
        }

        public void ApplyFromSync(List<SettingSyncDto> rows)
        {
            using AppDbContext db = new();

            foreach (SettingSyncDto row in rows)
            {
                if (!RecordSyncHelper.ShouldApplyRemoteRecord(db, "settings", row.Key, row.UpdatedAtUtc))
                    continue;

                SettingEntry? existing = db.Settings.SingleOrDefault(x => x.Key == row.Key);
                if (existing == null)
                    db.Settings.Add(new SettingEntry { Key = row.Key, Value = row.Value });
                else
                    existing.Value = row.Value;

                RecordSyncHelper.TouchRecordTimestamp(db, "settings", row.Key, row.UpdatedAtUtc);
            }

            db.SaveChanges();
        }
    }
}
