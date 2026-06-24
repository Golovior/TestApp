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
                db.Settings.Add(new SettingEntry
                {
                    Key = LegacyImportCompletedKey,
                    Value = "false"
                });

                db.SaveChanges();
            }
        }
    }
}