namespace TestApp
{
    internal static class Program
    {

        public static DataSetInfo dsI = new();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            _ = Program.GetInfo();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        public static DataSetInfo GetInfo()
        {
            return dsI;
        }

    }
}