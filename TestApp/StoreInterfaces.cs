namespace TestApp
{
    internal interface IGameStore
    {
        bool GameAlreadyExists(string name);
        List<string> GetAllGames();
        void AddGame(string name);
        string GetGameInfo();
        void UpdateFromApi(string data, long? remoteTimestamp = null);
    }

    internal interface IQuestionStore
    {
        bool QuestionAlreadyExists(string opdracht, string question);
        List<List<string>> GetAllQuestions();
        void AddQuestion(string opdracht, string question, string alphabetical);
        string GetQuestionInfo();
        void UpdateFromApi(string data, long? remoteTimestamp = null);
    }

    internal interface IAnswerStore
    {
        List<List<string>> GetAntwoorden();
        bool AntwoordAlreadyExists(string opdracht, string vraag, string name);
        void AddAntwoord(string opdracht, string vraag, string name, string correct = "0");
        string GetAntwoordenInfo();
        void SetAsCorrectAntwoord(string opdracht, string vraag, string name);
        void ConnectPlayersToAnswer(string opdracht, string vraag, string antwoord, List<string> spelers);
        void UpdateFromApi(string data, long? remoteTimestamp = null);
    }

    internal interface ISettingsStore
    {
        List<string> GetKeys();
        void UpdateSetting(string key, string value);
        string GetSettingsInfo();
        void UpdateFromApi(string data, long? remoteTimestamp = null);
    }

    internal interface ITestsStore
    {
        bool TestAlreadyExists(string test);
        List<string> GetAllTests();
        void AddTest(string test);
        void SetGameForTest(string test, string? game);
        string? GetGameForTest(string test);
        string GetTestInfo();
        void UpdateFromApi(string data, long? remoteTimestamp = null);
    }

    internal interface IOpdrachtenStore
    {
        List<string> GetOpdrachten();
        bool OpdrachtAlreadyExists(string name);
        void AddOpdracht(string name);
        string GetOpdrachtenInfo();
        void UpdateFromApi(string data, long? remoteTimestamp = null);
    }

    internal interface ISpelersStore
    {
        List<string> GetSpelers();
        bool SpelerAlreadyExists(string name);
        void AddSpeler(string name);
        string GetSpelersInfo();
        void UpdateFromApi(string data, long? remoteTimestamp = null);
    }

    internal interface ITestVragenStore
    {
        bool QuestionAlreadyExists(string test, string opdracht, string question);
        bool TestIsAfgenomen(string test);
        List<List<string>> GetAllTestVragen();
        void AddTestVraag(string test, string opdracht, string question, string order);
        void RemoveTestVragen(List<string> vraag);
        string GetTestVragenInfo();
        void UpdateFromApi(string data, long? remoteTimestamp = null);
    }

    internal interface ITestAntwoordenStore
    {
        bool TryAddTestAntwoord(Guid testAfnameId, string opdracht, string question, string antwoord);
        List<List<string>> GetAllTestAntwoorden();
        string GetTestAntwoordenInfo();
        void UpdateFromApi(string data, long? remoteTimestamp = null);
    }

    internal interface ITestAfnameStore
    {
        Guid StartAfname(string test, string speler);
        void EindeAfname(Guid id);
        List<List<string>> GetAllTestAfnamen();
    }

    internal interface IGameSpelersStore
    {
        List<List<string>> GetSpelersForGame(string game);
        void AssignSpelerToGame(string game, string speler);
        void RemoveSpelerFromGame(string game, string speler);
        void SetStatus(string game, string speler, string status);
    }
}