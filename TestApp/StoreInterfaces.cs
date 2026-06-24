namespace TestApp
{
    internal interface IGameStore
    {
        bool GameAlreadyExists(string name);
        void AddGame(string name);
        string GetGameInfo();
        void UpdateFromApi(string data);
    }

    internal interface IQuestionStore
    {
        bool QuestionAlreadyExists(string opdracht, string question);
        List<List<string>> GetAllQuestions();
        void AddQuestion(string opdracht, string question, string alphabetical);
        string GetQuestionInfo();
        void UpdateFromApi(string data);
    }

    internal interface IAnswerStore
    {
        List<List<string>> GetAntwoorden();
        bool AntwoordAlreadyExists(string opdracht, string vraag, string name);
        void AddAntwoord(string opdracht, string vraag, string name, string correct = "0");
        string GetAntwoordenInfo();
        void SetAsCorrectAntwoord(string opdracht, string vraag, string name);
        void ConnectPlayersToAnswer(string opdracht, string vraag, string antwoord, List<string> spelers);
        void UpdateFromApi(string data);
    }

    internal interface ISettingsStore
    {
        List<string> GetKeys();
        void UpdateSetting(string key, string value);
        string GetSettingsInfo();
        void UpdateFromApi(string data);
    }

    internal interface ITestsStore
    {
        bool TestAlreadyExists(string test);
        List<string> GetAllTests();
        void AddTest(string test);
        string GetTestInfo();
        void UpdateFromApi(string data);
    }

    internal interface IOpdrachtenStore
    {
        List<string> GetOpdrachten();
        bool OpdrachtAlreadyExists(string name);
        void AddOpdracht(string name);
        string GetOpdrachtenInfo();
        void UpdateFromApi(string data);
    }

    internal interface ISpelersStore
    {
        List<List<string>> GetSpelers();
        bool SpelerAlreadyExists(string name);
        void AddSpeler(string name);
        void SavePlayerStatus(string name, string status);
        string GetSpelersInfo();
        void UpdateFromApi(string data);
    }

    internal interface ITestVragenStore
    {
        bool QuestionAlreadyExists(string test, string opdracht, string question);
        List<List<string>> GetAllTestVragen();
        void AddTestVraag(string test, string opdracht, string question, string order);
        void RemoveTestVragen(List<string> vraag);
        string GetTestVragenInfo();
        void UpdateFromApi(string data);
    }

    internal interface ITestAntwoordenStore
    {
        bool AntwoordAlreadyExists(string test, string speler, string opdracht, string question, string antwoord);
        List<List<string>> GetAllTestAntwoorden();
        void AddTestAntwoord(string test, string speler, string opdracht, string question, string antwoord);
        void RemoveTestAntwoord(List<string> antwoord);
        string GetTestAntwoordenInfo();
        void UpdateFromApi(string data);
    }
}