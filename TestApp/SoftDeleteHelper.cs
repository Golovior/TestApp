using System.Linq;

namespace TestApp
{
    // Mirrors AppDbContext.cs's OnDelete cascade graph, but as soft-deletes: marking a row
    // Deleted=true instead of removing it, so the deletion can be propagated over sync
    // (see RecordSyncHelper). Every entry point runs inside the caller's own AppDbContext/
    // SaveChanges, so a whole cascade is one transaction.
    internal static class SoftDeleteHelper
    {
        public static void Opdracht(AppDbContext db, Guid opdrachtId, long ts)
        {
            Opdracht? entity = db.Opdrachten.SingleOrDefault(x => x.Id == opdrachtId);
            if (entity == null || entity.Deleted)
                return;

            entity.Deleted = true;
            RecordSyncHelper.TouchRecordTimestamp(db, "opdrachten", opdrachtId.ToString(), ts);

            foreach (Guid questionId in db.Questions.Where(x => x.OpdrachtId == opdrachtId).Select(x => x.Id).ToList())
                Question(db, questionId, ts);
        }

        public static void Question(AppDbContext db, Guid questionId, long ts)
        {
            Question? entity = db.Questions.SingleOrDefault(x => x.Id == questionId);
            if (entity == null || entity.Deleted)
                return;

            entity.Deleted = true;
            RecordSyncHelper.TouchRecordTimestamp(db, "questions", questionId.ToString(), ts);

            foreach (Guid answerId in db.Answers.Where(x => x.QuestionId == questionId).Select(x => x.Id).ToList())
                Answer(db, answerId, ts);

            foreach (Guid testQuestionId in db.TestQuestions.Where(x => x.QuestionId == questionId).Select(x => x.Id).ToList())
                TestQuestion(db, testQuestionId, ts);
        }

        public static void Answer(AppDbContext db, Guid answerId, long ts)
        {
            Answer? entity = db.Answers.SingleOrDefault(x => x.Id == answerId);
            if (entity == null || entity.Deleted)
                return;

            entity.Deleted = true;
            RecordSyncHelper.TouchRecordTimestamp(db, "antwoorden", answerId.ToString(), ts);

            foreach (Guid testAnswerId in db.TestAnswers.Where(x => x.AnswerId == answerId).Select(x => x.Id).ToList())
                TestAnswer(db, testAnswerId, ts);
        }

        public static void Test(AppDbContext db, Guid testId, long ts)
        {
            Test? entity = db.Tests.SingleOrDefault(x => x.Id == testId);
            if (entity == null || entity.Deleted)
                return;

            entity.Deleted = true;
            RecordSyncHelper.TouchRecordTimestamp(db, "tests", testId.ToString(), ts);

            foreach (Guid testQuestionId in db.TestQuestions.Where(x => x.TestId == testId).Select(x => x.Id).ToList())
                TestQuestion(db, testQuestionId, ts);

            foreach (Guid testAfnameId in db.TestAfnamen.Where(x => x.TestId == testId).Select(x => x.Id).ToList())
                TestAfname(db, testAfnameId, ts);
        }

        public static void TestQuestion(AppDbContext db, Guid testQuestionId, long ts)
        {
            TestQuestion? entity = db.TestQuestions.SingleOrDefault(x => x.Id == testQuestionId);
            if (entity == null || entity.Deleted)
                return;

            entity.Deleted = true;
            RecordSyncHelper.TouchRecordTimestamp(db, "testVragen", testQuestionId.ToString(), ts);

            foreach (Guid testAnswerId in db.TestAnswers.Where(x => x.TestQuestionId == testQuestionId).Select(x => x.Id).ToList())
                TestAnswer(db, testAnswerId, ts);
        }

        // Game -> Tests is DeleteBehavior.SetNull (not Cascade) in AppDbContext.cs, so a
        // deleted game orphans its tests' GameId rather than soft-deleting them too.
        public static void Game(AppDbContext db, Guid gameId, long ts)
        {
            Game? entity = db.Games.SingleOrDefault(x => x.Id == gameId);
            if (entity == null || entity.Deleted)
                return;

            entity.Deleted = true;
            RecordSyncHelper.TouchRecordTimestamp(db, "games", gameId.ToString(), ts);

            foreach (Guid gameSpelerId in db.GameSpelers.Where(x => x.GameId == gameId).Select(x => x.Id).ToList())
                GameSpeler(db, gameSpelerId, ts);

            foreach (Test test in db.Tests.Where(x => x.GameId == gameId).ToList())
            {
                test.GameId = null;
                RecordSyncHelper.TouchRecordTimestamp(db, "tests", test.Id.ToString(), ts);
            }
        }

        public static void Player(AppDbContext db, Guid playerId, long ts)
        {
            Player? entity = db.Players.SingleOrDefault(x => x.Id == playerId);
            if (entity == null || entity.Deleted)
                return;

            entity.Deleted = true;
            RecordSyncHelper.TouchRecordTimestamp(db, "spelers", playerId.ToString(), ts);

            foreach (Guid gameSpelerId in db.GameSpelers.Where(x => x.SpelerId == playerId).Select(x => x.Id).ToList())
                GameSpeler(db, gameSpelerId, ts);

            foreach (Guid testAfnameId in db.TestAfnamen.Where(x => x.SpelerId == playerId).Select(x => x.Id).ToList())
                TestAfname(db, testAfnameId, ts);
        }

        public static void TestAfname(AppDbContext db, Guid testAfnameId, long ts)
        {
            TestAfname? entity = db.TestAfnamen.SingleOrDefault(x => x.Id == testAfnameId);
            if (entity == null || entity.Deleted)
                return;

            entity.Deleted = true;
            RecordSyncHelper.TouchRecordTimestamp(db, "testAfnamen", testAfnameId.ToString(), ts);

            foreach (Guid testAnswerId in db.TestAnswers.Where(x => x.TestAfnameId == testAfnameId).Select(x => x.Id).ToList())
                TestAnswer(db, testAnswerId, ts);
        }

        public static void GameSpeler(AppDbContext db, Guid gameSpelerId, long ts)
        {
            GameSpeler? entity = db.GameSpelers.SingleOrDefault(x => x.Id == gameSpelerId);
            if (entity == null || entity.Deleted)
                return;

            entity.Deleted = true;
            RecordSyncHelper.TouchRecordTimestamp(db, "gameSpelers", gameSpelerId.ToString(), ts);
        }

        public static void TestAnswer(AppDbContext db, Guid testAnswerId, long ts)
        {
            TestAnswer? entity = db.TestAnswers.SingleOrDefault(x => x.Id == testAnswerId);
            if (entity == null || entity.Deleted)
                return;

            entity.Deleted = true;
            RecordSyncHelper.TouchRecordTimestamp(db, "testAntwoorden", testAnswerId.ToString(), ts);
        }
    }
}
