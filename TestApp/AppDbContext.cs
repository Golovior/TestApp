using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Game> Games => Set<Game>();
        public DbSet<SettingEntry> Settings => Set<SettingEntry>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Answer> Answers => Set<Answer>();
        public DbSet<Test> Tests => Set<Test>();
        public DbSet<TestQuestion> TestQuestions => Set<TestQuestion>();
        public DbSet<TestAnswer> TestAnswers => Set<TestAnswer>();
        public DbSet<TestAfname> TestAfnamen => Set<TestAfname>();
        public DbSet<GameSpeler> GameSpelers => Set<GameSpeler>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Opdracht> Opdrachten => Set<Opdracht>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite($"Data Source={AppStoragePaths.DatabaseFile}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<SettingEntry>(entity =>
            {
                entity.HasKey(e => e.Key);
                entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Value).IsRequired().HasMaxLength(1000);
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OpdrachtId).IsRequired();
                entity.Property(e => e.Text).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Alphabetical).IsRequired().HasMaxLength(200);
                entity.HasOne(e => e.Opdracht)
                    .WithMany(o => o.Questions)
                    .HasForeignKey(e => e.OpdrachtId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.OpdrachtId, e.Text }).IsUnique();
            });

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QuestionId).IsRequired();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.ConnectedPlayersJson).IsRequired().HasMaxLength(4000);
                entity.HasOne(e => e.Question)
                    .WithMany(q => q.Answers)
                    .HasForeignKey(e => e.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.QuestionId, e.Name }).IsUnique();
            });

            modelBuilder.Entity<Test>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasOne(e => e.Game)
                    .WithMany(g => g.Tests)
                    .HasForeignKey(e => e.GameId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TestQuestion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TestId).IsRequired();
                entity.Property(e => e.QuestionId).IsRequired();
                entity.Property(e => e.Order).IsRequired().HasMaxLength(50);
                entity.HasOne(e => e.Test)
                    .WithMany(t => t.TestQuestions)
                    .HasForeignKey(e => e.TestId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Question)
                    .WithMany(q => q.TestQuestions)
                    .HasForeignKey(e => e.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.TestId, e.QuestionId, e.Order }).IsUnique();
            });

            modelBuilder.Entity<TestAnswer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TestAfnameId).IsRequired();
                entity.Property(e => e.TestQuestionId).IsRequired();
                entity.Property(e => e.AnswerId).IsRequired();
                entity.HasOne(e => e.TestAfname)
                    .WithMany(a => a.TestAnswers)
                    .HasForeignKey(e => e.TestAfnameId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.TestQuestion)
                    .WithMany(q => q.TestAnswers)
                    .HasForeignKey(e => e.TestQuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Answer)
                    .WithMany(a => a.TestAnswers)
                    .HasForeignKey(e => e.AnswerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.TestAfnameId, e.TestQuestionId }).IsUnique();
            });

            modelBuilder.Entity<TestAfname>(entity =>
            {
                entity.ToTable("TestAfname");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TestId).IsRequired();
                entity.Property(e => e.SpelerId).IsRequired();
                entity.Property(e => e.Starttijd).IsRequired();
                entity.HasOne(e => e.Test)
                    .WithMany(t => t.TestAfnamen)
                    .HasForeignKey(e => e.TestId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Speler)
                    .WithMany()
                    .HasForeignKey(e => e.SpelerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<GameSpeler>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.GameId).IsRequired();
                entity.Property(e => e.SpelerId).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.HasOne(e => e.Game)
                    .WithMany(g => g.GameSpelers)
                    .HasForeignKey(e => e.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Speler)
                    .WithMany()
                    .HasForeignKey(e => e.SpelerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.GameId, e.SpelerId }).IsUnique();
            });

            modelBuilder.Entity<Opdracht>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            });
        }
    }
}