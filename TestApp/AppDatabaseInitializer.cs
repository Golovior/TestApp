namespace TestApp
{
    internal static class AppDatabaseInitializer
    {
        private const string LegacyImportCompletedKey = "LegacyImportCompleted";

        public static void Initialize()
        {
            using AppDbContext db = new();

            db.Database.EnsureCreated();

            SettingEntry? marker = db.Settings.Find(LegacyImportCompletedKey);
            if (marker == null)
            {
                marker = new SettingEntry
                {
                    Key = LegacyImportCompletedKey,
                    Value = "false"
                };

                db.Settings.Add(marker);

                db.SaveChanges();
            }

            if (!string.Equals(marker.Value, "true", StringComparison.OrdinalIgnoreCase))
            {
                LegacyDataImporter importer = new();
                importer.ImportFromLegacyFiles();

                marker.Value = "true";
                db.SaveChanges();
            }
        }
    }
}