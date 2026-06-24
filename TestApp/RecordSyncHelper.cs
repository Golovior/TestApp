using System.Text;

namespace TestApp
{
    internal static class RecordSyncHelper
    {
        private static string EncodePart(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            return Convert.ToBase64String(bytes);
        }

        public static string BuildRecordKey(string tableName, params string[] parts)
        {
            string joined = string.Join(":", parts.Select(EncodePart));
            return $"SyncRecord:{tableName}:{joined}";
        }

        public static long GetCurrentUnixTimeSeconds() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public static bool ShouldApplyRemoteRecord(AppDbContext db, string tableName, string[] keyParts, long? remoteTimestamp)
        {
            if (!remoteTimestamp.HasValue)
                return true;

            string key = BuildRecordKey(tableName, keyParts);
            SettingEntry? entry = db.Settings.SingleOrDefault(x => x.Key == key);

            if (entry == null)
                return true;

            if (!long.TryParse(entry.Value, out long localTimestamp))
                return true;

            return remoteTimestamp.Value >= localTimestamp;
        }

        public static void TouchRecordTimestamp(AppDbContext db, string tableName, string[] keyParts, long? timestamp = null)
        {
            long value = timestamp ?? GetCurrentUnixTimeSeconds();
            string key = BuildRecordKey(tableName, keyParts);
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