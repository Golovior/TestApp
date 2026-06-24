namespace TestApp
{
    internal class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    internal class SettingEntry
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    internal class Question
    {
        public int Id { get; set; }
        public string Opdracht { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Alphabetical { get; set; } = string.Empty;
    }

    internal class Answer
    {
        public int Id { get; set; }
        public string Opdracht { get; set; } = string.Empty;
        public string Vraag { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string ConnectedPlayersJson { get; set; } = "[]";
    }

    internal class Test
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    internal class TestQuestion
    {
        public int Id { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string Opdracht { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string Order { get; set; } = string.Empty;
    }

    internal class TestAnswer
    {
        public int Id { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string Speler { get; set; } = string.Empty;
        public string Opdracht { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string AnswerText { get; set; } = string.Empty;
    }

    internal class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
    }

    internal class Opdracht
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}