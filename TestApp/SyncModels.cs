namespace TestApp
{
    // One flat DTO per table, mirroring AppEntities.cs exactly: the entity's real
    // Id/foreign-key Guids plus its scalar fields plus UpdatedAtUtc (Unix seconds),
    // matching whatever RecordSyncHelper has stored for that row. See
    // SYNC_CONTRACT.md for the full wire-format specification.

    public class GameSyncDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public bool Deleted { get; set; }
        public long UpdatedAtUtc { get; set; }
    }

    // SettingEntry's real primary key is its Key string, not a Guid.
    public class SettingSyncDto
    {
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
        public long UpdatedAtUtc { get; set; }
    }

    public class OpdrachtSyncDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public bool Deleted { get; set; }
        public long UpdatedAtUtc { get; set; }
    }

    public class QuestionSyncDto
    {
        public Guid Id { get; set; }
        public Guid OpdrachtId { get; set; }
        public string Text { get; set; } = "";
        public string Alphabetical { get; set; } = "";
        public bool Deleted { get; set; }
        public long UpdatedAtUtc { get; set; }
    }

    public class AnswerSyncDto
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string Name { get; set; } = "";
        public bool IsCorrect { get; set; }
        // Still player *names* (not ids) - matches ConnectPlayersToAnswer's
        // existing on-disk representation, out of scope for this redesign.
        public string ConnectedPlayersJson { get; set; } = "[]";
        public bool Deleted { get; set; }
        public long UpdatedAtUtc { get; set; }
    }

    public class TestSyncDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public Guid? GameId { get; set; }
        public bool Deleted { get; set; }
        public long UpdatedAtUtc { get; set; }
    }

    public class TestQuestionSyncDto
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public Guid QuestionId { get; set; }
        public string Order { get; set; } = "";
        public bool Deleted { get; set; }
        public long UpdatedAtUtc { get; set; }
    }

    public class PlayerSyncDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public bool Deleted { get; set; }
        public long UpdatedAtUtc { get; set; }
    }

    public class GameSpelerSyncDto
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Guid SpelerId { get; set; }
        public string Status { get; set; } = "1";
        public bool Deleted { get; set; }
        public long UpdatedAtUtc { get; set; }
    }

    public class TestAfnameSyncDto
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public Guid SpelerId { get; set; }
        public DateTime Starttijd { get; set; }
        public DateTime? Eindtijd { get; set; }
        public int? Jokers { get; set; }
        public bool Deleted { get; set; }
        public long UpdatedAtUtc { get; set; }
    }

    public class TestAnswerSyncDto
    {
        public Guid Id { get; set; }
        public Guid TestAfnameId { get; set; }
        public Guid TestQuestionId { get; set; }
        public Guid AnswerId { get; set; }
        public bool Deleted { get; set; }
        public long UpdatedAtUtc { get; set; }
    }

    // Used for both the outgoing request and the incoming response - the shape
    // is fully symmetric, unlike the old text-tuple format.
    public class SyncEnvelope
    {
        public List<GameSyncDto> Games { get; set; } = new();
        public List<SettingSyncDto> Settings { get; set; } = new();
        public List<OpdrachtSyncDto> Opdrachten { get; set; } = new();
        public List<QuestionSyncDto> Questions { get; set; } = new();
        public List<AnswerSyncDto> Answers { get; set; } = new();
        public List<TestSyncDto> Tests { get; set; } = new();
        public List<TestQuestionSyncDto> TestQuestions { get; set; } = new();
        public List<PlayerSyncDto> Players { get; set; } = new();
        public List<GameSpelerSyncDto> GameSpelers { get; set; } = new();
        public List<TestAfnameSyncDto> TestAfnamen { get; set; } = new();
        public List<TestAnswerSyncDto> TestAnswers { get; set; } = new();
    }
}
