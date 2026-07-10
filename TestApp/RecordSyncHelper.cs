using System.Text;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal static class RecordSyncHelper
    {
        private static string EncodePart(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            return Convert.ToBase64String(bytes);
        }

        // "recordKey" is the row's real identity: a Guid.ToString() for every
        // Guid-keyed table, or the raw Key string for the SettingEntry table
        // (whose primary key is a string, not a Guid).
        public static string BuildRecordKey(string tableName, string recordKey)
        {
            return $"SyncRecord:{tableName}:{EncodePart(recordKey)}";
        }

        public static long GetCurrentUnixTimeSeconds() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public static long? GetRecordTimestamp(AppDbContext db, string tableName, string recordKey)
        {
            string key = BuildRecordKey(tableName, recordKey);
            SettingEntry? entry = db.Settings.AsNoTracking().SingleOrDefault(x => x.Key == key);

            if (entry == null || !long.TryParse(entry.Value, out long localTimestamp))
                return null;

            return localTimestamp;
        }

        public static bool ShouldApplyRemoteRecord(AppDbContext db, string tableName, string recordKey, long? remoteTimestamp)
        {
            if (!remoteTimestamp.HasValue)
                return true;

            long? localTimestamp = GetRecordTimestamp(db, tableName, recordKey);

            if (!localTimestamp.HasValue)
                return true;

            return remoteTimestamp.Value >= localTimestamp.Value;
        }

        public static void TouchRecordTimestamp(AppDbContext db, string tableName, string recordKey, long? timestamp = null)
        {
            long value = timestamp ?? GetCurrentUnixTimeSeconds();
            string key = BuildRecordKey(tableName, recordKey);
            SettingEntry? entry = db.Settings.SingleOrDefault(x => x.Key == key);

            if (entry == null)
            {
                db.Settings.Add(new SettingEntry
                {
                    Key = key,
                    Value = value.ToString()
                });
            }
            else
            {
                entry.Value = value.ToString();
            }
        }
    }
}