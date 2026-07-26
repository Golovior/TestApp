namespace TestApp
{
    internal interface IGameStore
    {
        bool GameAlreadyExists(string name);
        List<string> GetAllGames();
        void AddGame(string name);
        void DeleteGame(string name);
        List<GameSyncDto> GetForSync();
        void ApplyFromSync(List<GameSyncDto> rows);
    }

    internal interface IQuestionStore
    {
        bool QuestionAlreadyExists(string opdracht, string question);
        List<List<string>> GetAllQuestions();
        void AddQuestion(string opdracht, string question, string alphabetical);
        void SetAlphabetical(string opdracht, string question, string alphabetical);
        void DeleteQuestion(string opdracht, string question);
        List<QuestionSyncDto> GetForSync();
        void ApplyFromSync(List<QuestionSyncDto> rows);
    }

    internal interface IAnswerStore
    {
        List<List<string>> GetAntwoorden();
        bool AntwoordAlreadyExists(string opdracht, string vraag, string name);
        void AddAntwoord(string opdracht, string vraag, string name, string correct = "0");
        void SetAsCorrectAntwoord(string opdracht, string vraag, string name);
        void ConnectPlayersToAnswer(string opdracht, string vraag, string antwoord, List<string> spelers);
        void DeleteAntwoord(string opdracht, string vraag, string naam);
        List<AnswerSyncDto> GetForSync();
        void ApplyFromSync(List<AnswerSyncDto> rows);
    }

    internal interface ISettingsStore
    {
        List<string> GetKeys();
        void UpdateSetting(string key, string value);
        List<SettingSyncDto> GetForSync();
        void ApplyFromSync(List<SettingSyncDto> rows);
    }

    internal interface ITestsStore
    {
        bool TestAlreadyExists(string test);
        List<string> GetAllTests();
        void AddTest(string test);
        void SetGameForTest(string test, string? game);
        string? GetGameForTest(string test);
        void DeleteTest(string test);
        List<TestSyncDto> GetForSync();
        void ApplyFromSync(List<TestSyncDto> rows);
    }

    internal interface IOpdrachtenStore
    {
        List<string> GetOpdrachten();
        bool OpdrachtAlreadyExists(string name);
        void AddOpdracht(string name);
        void DeleteOpdracht(string name);
        List<OpdrachtSyncDto> GetForSync();
        void ApplyFromSync(List<OpdrachtSyncDto> rows);
    }

    internal interface ISpelersStore
    {
        List<string> GetSpelers();
        bool SpelerAlreadyExists(string name);
        void AddSpeler(string name);
        void DeleteSpeler(string name);
        List<PlayerSyncDto> GetForSync();
        void ApplyFromSync(List<PlayerSyncDto> rows);
    }

    internal interface ITestVragenStore
    {
        bool QuestionAlreadyExists(string test, string opdracht, string question);
        bool TestIsAfgenomen(string test);
        List<List<string>> GetAllTestVragen();
        void AddTestVraag(string test, string opdracht, string question, string order);
        void RemoveTestVragen(List<string> vraag);
        List<TestQuestionSyncDto> GetForSync();
        void ApplyFromSync(List<TestQuestionSyncDto> rows);
    }

    internal interface ITestAntwoordenStore
    {
        bool TryAddTestAntwoord(Guid testAfnameId, string opdracht, string question, string antwoord);
        List<List<string>> GetAllTestAntwoorden();
        void DeleteTestAntwoord(Guid id);
        List<TestAnswerSyncDto> GetForSync();
        void ApplyFromSync(List<TestAnswerSyncDto> rows);
    }

    internal interface ITestAfnameStore
    {
        Guid StartAfname(string test, string speler);
        void EindeAfname(Guid id);
        List<List<string>> GetAllTestAfnamen();
        void DeleteTestAfname(Guid id);
        List<TestAfnameSyncDto> GetForSync();
        void ApplyFromSync(List<TestAfnameSyncDto> rows);
    }

    internal interface IGameSpelersStore
    {
        List<List<string>> GetSpelersForGame(string game);
        void AssignSpelerToGame(string game, string speler);
        void RemoveSpelerFromGame(string game, string speler);
        void SetStatus(string game, string speler, string status);
        List<GameSpelerSyncDto> GetForSync();
        void ApplyFromSync(List<GameSpelerSyncDto> rows);
    }
}
