using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal static class AppDatabaseInitializer
    {
        private const string LegacyImportCompletedKey = "LegacyImportCompleted";

        public static void Initialize()
        {
            using AppDbContext db = new();

            db.Database.EnsureCreated();
            EnsureTestAfnameTableExists(db);
            EnsureTestAnswersTableIsCurrentShape(db);
            EnsureGameSpelersTableExists(db);
            EnsureTestsHaveGameIdColumn(db);
            EnsurePlayersStatusColumnRemoved(db);
            EnsureDeletedColumnsExist(db);
            EnsureFilteredUniqueIndexes(db);

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

        // EnsureCreated() only creates the schema for a brand-new database file; it never
        // adds tables to a database that already exists. This keeps existing installs working
        // after the TestAfname table was added to the model.
        private static void EnsureTestAfnameTableExists(AppDbContext db)
        {
            db.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS ""TestAfname"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_TestAfname"" PRIMARY KEY,
                    ""TestId"" TEXT NOT NULL,
                    ""SpelerId"" TEXT NOT NULL,
                    ""Starttijd"" TEXT NOT NULL,
                    ""Eindtijd"" TEXT NULL,
                    ""Jokers"" INTEGER NULL,
                    CONSTRAINT ""FK_TestAfname_Players_SpelerId"" FOREIGN KEY (""SpelerId"") REFERENCES ""Players"" (""Id"") ON DELETE CASCADE,
                    CONSTRAINT ""FK_TestAfname_Tests_TestId"" FOREIGN KEY (""TestId"") REFERENCES ""Tests"" (""Id"") ON DELETE CASCADE
                );");

            db.Database.ExecuteSqlRaw(@"CREATE INDEX IF NOT EXISTS ""IX_TestAfname_TestId"" ON ""TestAfname"" (""TestId"");");
            db.Database.ExecuteSqlRaw(@"CREATE INDEX IF NOT EXISTS ""IX_TestAfname_SpelerId"" ON ""TestAfname"" (""SpelerId"");");
        }

        private static bool TableHasColumn(AppDbContext db, string table, string column)
        {
            var connection = db.Database.GetDbConnection();
            bool shouldClose = connection.State != System.Data.ConnectionState.Open;
            if (shouldClose)
                connection.Open();

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = $"PRAGMA table_info('{table}')";
                using var reader = command.ExecuteReader();
                int nameOrdinal = -1;

                while (reader.Read())
                {
                    if (nameOrdinal < 0)
                        nameOrdinal = reader.GetOrdinal("name");

                    if (string.Equals(reader.GetString(nameOrdinal), column, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                return false;
            }
            finally
            {
                if (shouldClose)
                    connection.Close();
            }
        }

        // TestAnswer was redesigned to hang off TestAfname/TestQuestion/Answer instead of
        // loose Test/Player ids and free-text columns. There is no migration for the old
        // shape: by product decision, any TestAnswers rows from before this change are
        // dropped rather than migrated.
        private static void EnsureTestAnswersTableIsCurrentShape(AppDbContext db)
        {
            bool tableExists = TableHasColumn(db, "TestAnswers", "Id");
            bool hasCurrentShape = TableHasColumn(db, "TestAnswers", "TestAfnameId");

            if (tableExists && !hasCurrentShape)
                db.Database.ExecuteSqlRaw(@"DROP TABLE ""TestAnswers"";");

            db.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS ""TestAnswers"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_TestAnswers"" PRIMARY KEY,
                    ""TestAfnameId"" TEXT NOT NULL,
                    ""TestQuestionId"" TEXT NOT NULL,
                    ""AnswerId"" TEXT NOT NULL,
                    CONSTRAINT ""FK_TestAnswers_TestAfname_TestAfnameId"" FOREIGN KEY (""TestAfnameId"") REFERENCES ""TestAfname"" (""Id"") ON DELETE CASCADE,
                    CONSTRAINT ""FK_TestAnswers_TestQuestions_TestQuestionId"" FOREIGN KEY (""TestQuestionId"") REFERENCES ""TestQuestions"" (""Id"") ON DELETE CASCADE,
                    CONSTRAINT ""FK_TestAnswers_Answers_AnswerId"" FOREIGN KEY (""AnswerId"") REFERENCES ""Answers"" (""Id"") ON DELETE CASCADE
                );");

            db.Database.ExecuteSqlRaw(@"CREATE UNIQUE INDEX IF NOT EXISTS ""IX_TestAnswers_TestAfnameId_TestQuestionId"" ON ""TestAnswers"" (""TestAfnameId"", ""TestQuestionId"");");
            db.Database.ExecuteSqlRaw(@"CREATE INDEX IF NOT EXISTS ""IX_TestAnswers_TestQuestionId"" ON ""TestAnswers"" (""TestQuestionId"");");
            db.Database.ExecuteSqlRaw(@"CREATE INDEX IF NOT EXISTS ""IX_TestAnswers_AnswerId"" ON ""TestAnswers"" (""AnswerId"");");
        }

        private static void EnsureGameSpelersTableExists(AppDbContext db)
        {
            db.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS ""GameSpelers"" (
                    ""Id"" TEXT NOT NULL CONSTRAINT ""PK_GameSpelers"" PRIMARY KEY,
                    ""GameId"" TEXT NOT NULL,
                    ""SpelerId"" TEXT NOT NULL,
                    ""Status"" TEXT NOT NULL,
                    CONSTRAINT ""FK_GameSpelers_Games_GameId"" FOREIGN KEY (""GameId"") REFERENCES ""Games"" (""Id"") ON DELETE CASCADE,
                    CONSTRAINT ""FK_GameSpelers_Players_SpelerId"" FOREIGN KEY (""SpelerId"") REFERENCES ""Players"" (""Id"") ON DELETE CASCADE
                );");

            db.Database.ExecuteSqlRaw(@"CREATE UNIQUE INDEX IF NOT EXISTS ""IX_GameSpelers_GameId_SpelerId"" ON ""GameSpelers"" (""GameId"", ""SpelerId"");");
            db.Database.ExecuteSqlRaw(@"CREATE INDEX IF NOT EXISTS ""IX_GameSpelers_SpelerId"" ON ""GameSpelers"" (""SpelerId"");");
        }

        // Additive nullable column: SQLite allows ADD COLUMN with a REFERENCES clause without
        // a rebuild, and since every existing row gets NULL, no existing data can violate it.
        private static void EnsureTestsHaveGameIdColumn(AppDbContext db)
        {
            if (TableHasColumn(db, "Tests", "GameId"))
                return;

            db.Database.ExecuteSqlRaw(@"ALTER TABLE ""Tests"" ADD COLUMN ""GameId"" TEXT NULL REFERENCES ""Games"" (""Id"");");
            db.Database.ExecuteSqlRaw(@"CREATE INDEX IF NOT EXISTS ""IX_Tests_GameId"" ON ""Tests"" (""GameId"");");
        }

        // Player.Status is NOT NULL with no default in the existing physical table, so once
        // EF stops setting it, every insert fails unless the column is actually dropped
        // (not just left unmapped).
        private static void EnsurePlayersStatusColumnRemoved(AppDbContext db)
        {
            if (TableHasColumn(db, "Players", "Status"))
                db.Database.ExecuteSqlRaw(@"ALTER TABLE ""Players"" DROP COLUMN ""Status"";");
        }

        // Soft-delete support: every domain table (everything except Settings, which is
        // app config / sync bookkeeping rather than a user-facing record) gets a Deleted
        // flag. NOT NULL DEFAULT 0 means every existing row becomes "not deleted" for free.
        private static readonly string[] DeletableTables =
        {
            "Games", "Opdrachten", "Questions", "Answers", "Tests",
            "TestQuestions", "Players", "GameSpelers", "TestAfname", "TestAnswers"
        };

        private static void EnsureDeletedColumnsExist(AppDbContext db)
        {
            foreach (string table in DeletableTables)
            {
                if (!TableHasColumn(db, table, "Deleted"))
                    db.Database.ExecuteSqlRaw($@"ALTER TABLE ""{table}"" ADD COLUMN ""Deleted"" INTEGER NOT NULL DEFAULT 0;");
            }
        }

        // These indexes were plain unique indexes before soft-delete existed, so on an
        // upgraded install the old (non-filtered) index is still physically present and
        // would block re-adding a row with the same key as a deleted one. Dropping and
        // recreating as a filtered index is idempotent, so it's safe to run every startup.
        private static void EnsureFilteredUniqueIndexes(AppDbContext db)
        {
            (string indexName, string table, string columns)[] indexes =
            {
                ("IX_Games_Name", "Games", "\"Name\""),
                ("IX_Tests_Name", "Tests", "\"Name\""),
                ("IX_Questions_OpdrachtId_Text", "Questions", "\"OpdrachtId\", \"Text\""),
                ("IX_Answers_QuestionId_Name", "Answers", "\"QuestionId\", \"Name\""),
                ("IX_TestQuestions_TestId_QuestionId_Order", "TestQuestions", "\"TestId\", \"QuestionId\", \"Order\""),
                ("IX_GameSpelers_GameId_SpelerId", "GameSpelers", "\"GameId\", \"SpelerId\""),
                ("IX_TestAnswers_TestAfnameId_TestQuestionId", "TestAnswers", "\"TestAfnameId\", \"TestQuestionId\"")
            };

            foreach ((string indexName, string table, string columns) in indexes)
            {
                db.Database.ExecuteSqlRaw($@"DROP INDEX IF EXISTS ""{indexName}"";");
                db.Database.ExecuteSqlRaw($@"CREATE UNIQUE INDEX IF NOT EXISTS ""{indexName}"" ON ""{table}"" ({columns}) WHERE ""Deleted"" = 0;");
            }
        }
    }
}