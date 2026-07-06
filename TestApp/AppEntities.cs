namespace TestApp
{
    internal class Game
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Test> Tests { get; set; } = new();
        public List<GameSpeler> GameSpelers { get; set; } = new();
    }

    internal class SettingEntry
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    internal class Question
    {
        public Guid Id { get; set; }
        public Guid OpdrachtId { get; set; }
        public Opdracht Opdracht { get; set; } = null!;
        public string Text { get; set; } = string.Empty;
        public string Alphabetical { get; set; } = string.Empty;
        public List<Answer> Answers { get; set; } = new();
        public List<TestQuestion> TestQuestions { get; set; } = new();
    }

    internal class Answer
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string ConnectedPlayersJson { get; set; } = "[]";
        public List<TestAnswer> TestAnswers { get; set; } = new();
    }

    internal class Test
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? GameId { get; set; }
        public Game? Game { get; set; }
        public List<TestQuestion> TestQuestions { get; set; } = new();
        public List<TestAfname> TestAfnamen { get; set; } = new();
    }

    internal class GameSpeler
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Game Game { get; set; } = null!;
        public Guid SpelerId { get; set; }
        public Player Speler { get; set; } = null!;
        public string Status { get; set; } = "1";
    }

    internal class TestAfname
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public Test Test { get; set; } = null!;
        public Guid SpelerId { get; set; }
        public Player Speler { get; set; } = null!;
        public DateTime Starttijd { get; set; }
        public DateTime? Eindtijd { get; set; }
        public int? Jokers { get; set; }
        public List<TestAnswer> TestAnswers { get; set; } = new();
    }

    internal class TestQuestion
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public Test Test { get; set; } = null!;
        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;
        public string Order { get; set; } = string.Empty;
        public List<TestAnswer> TestAnswers { get; set; } = new();
    }

    internal class TestAnswer
    {
        public Guid Id { get; set; }
        public Guid TestAfnameId { get; set; }
        public TestAfname TestAfname { get; set; } = null!;
        public Guid TestQuestionId { get; set; }
        public TestQuestion TestQuestion { get; set; } = null!;
        public Guid AnswerId { get; set; }
        public Answer Answer { get; set; } = null!;
    }

    internal class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    internal class Opdracht
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Question> Questions { get; set; } = new();
    }
}