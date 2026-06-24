namespace TestApp
{
    internal class Game
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
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
        public List<TestQuestion> TestQuestions { get; set; } = new();
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
    }

    internal class TestAnswer
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public Test Test { get; set; } = null!;
        public Guid PlayerId { get; set; }
        public Player Player { get; set; } = null!;
        public Guid? AnswerId { get; set; }
        public Answer? Answer { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string AnswerText { get; set; } = string.Empty;
    }

    internal class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
    }

    internal class Opdracht
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Question> Questions { get; set; } = new();
    }
}