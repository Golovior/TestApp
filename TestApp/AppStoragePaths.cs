namespace TestApp
{
    internal static class AppStoragePaths
    {
        public static string DataDirectory => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "widmTest");

        public static string DatabaseFile => Path.Combine(DataDirectory, "app.db");
    }
}
