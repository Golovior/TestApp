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
                entity.Property(e => e.Opdracht).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Text).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Alphabetical).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => new { e.Opdracht, e.Text }).IsUnique();
            });

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Opdracht).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Vraag).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.ConnectedPlayersJson).IsRequired().HasMaxLength(4000);
                entity.HasIndex(e => new { e.Opdracht, e.Vraag, e.Name }).IsUnique();
            });

            modelBuilder.Entity<Test>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<TestQuestion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TestName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Opdracht).IsRequired().HasMaxLength(200);
                entity.Property(e => e.QuestionText).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Order).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<TestAnswer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TestName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Speler).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Opdracht).IsRequired().HasMaxLength(200);
                entity.Property(e => e.QuestionText).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.AnswerText).IsRequired().HasMaxLength(1000);
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            });

            modelBuilder.Entity<Opdracht>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            });
        }
    }
}