namespace TestApp
{
    internal static class Program
    {

        public static DataSetClass ds = new();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            AppDatabaseInitializer.Initialize();
            Application.Run(new Main());
        }

        public static DataSetClass GetInfo()
        {
            return ds;
        }

    }
}