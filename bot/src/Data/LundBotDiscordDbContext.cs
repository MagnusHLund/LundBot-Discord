using LundBot.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LundBot.Data
{
    public sealed class LundBotDiscordDbContext : DbContext
    {
        public DbSet<LeaderboardsEntity> Leaderboards { get; set; } = null!;
        public DbSet<LeaderboardScoresEntity> LeaderboardScores { get; set; } = null!;
        public DbSet<LeaderboardScoreSourceEntity> LeaderboardScoreSources { get; set; } = null!;
        public DbSet<LeaderboardMessagesEntity> LeaderboardMessages { get; set; } = null!;
        public DbSet<WebsiteTrafficEntity> WebsiteTraffic { get; set; } = null!;
        public DbSet<WebsiteTrafficMessagesEntity> WebsiteTrafficMessages { get; set; } = null!;

        public LundBotDiscordDbContext(DbContextOptions<LundBotDiscordDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureLeaderboards(modelBuilder.Entity<LeaderboardsEntity>());
            ConfigureLeaderboardScores(modelBuilder.Entity<LeaderboardScoresEntity>());
            ConfigureLeaderBoardScoreSource(modelBuilder.Entity<LeaderboardScoreSourceEntity>());
            ConfigureLeaderboardMessages(modelBuilder.Entity<LeaderboardMessagesEntity>());
            ConfigureWebsiteTraffic(modelBuilder.Entity<WebsiteTrafficEntity>());
            ConfigureWebsiteTrafficMessages(modelBuilder.Entity<WebsiteTrafficMessagesEntity>());
        }

        private static void ConfigureLeaderboards(EntityTypeBuilder<LeaderboardsEntity> entity)
        {
            entity.ToTable("Leaderboards");

            entity.HasKey(e => e.Id);
            entity
                .Property(e => e.Id)
                .HasColumnName("LeaderboardsId")
                .HasColumnType("int unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.DiscordServerId).HasColumnType("char(19)").IsRequired();

            entity.Property(e => e.DiscordChannelId).HasColumnType("char(19)").IsRequired();

            entity.Property(e => e.Title).HasColumnType("varchar(64)").IsRequired();

            entity.Property(e => e.Message).HasColumnType("varchar(256)").IsRequired();

            entity
                .Property(e => e.LeaderboardType)
                .HasConversion<string>()
                .HasColumnType("varchar(32)")
                .IsRequired();

            entity
                .Property(e => e.CreatedAt)
                .HasColumnType("datetime(3)")
                .HasDefaultValueSql("UTC_TIMESTAMP(3)");

            entity
                .HasIndex(
                    nameof(LeaderboardsEntity.DiscordServerId),
                    nameof(LeaderboardsEntity.DiscordChannelId)
                )
                .IsUnique()
                .HasDatabaseName("leaderboards_index_2");

            entity
                .HasIndex(
                    nameof(LeaderboardsEntity.DiscordServerId),
                    nameof(LeaderboardsEntity.LeaderboardType)
                )
                .HasDatabaseName("leaderboards_index_3");

            entity.HasIndex(e => e.LeaderboardType);
        }

        private static void ConfigureLeaderboardScores(
            EntityTypeBuilder<LeaderboardScoresEntity> entity
        )
        {
            entity.ToTable("LeaderboardScores");

            entity.HasKey(e => e.Id);
            entity
                .Property(e => e.Id)
                .HasColumnName("LeaderboardScoresId")
                .HasColumnType("int unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.LeaderboardsId).HasColumnType("int unsigned").IsRequired();

            entity.Property(e => e.DiscordUserId).HasColumnType("char(19)").IsRequired();

            entity.Property(e => e.Score).HasColumnType("int unsigned").IsRequired();

            entity
                .Property(e => e.UpdatedAt)
                .HasColumnType("datetime(3)")
                .ValueGeneratedOnAddOrUpdate();

            entity
                .Property(e => e.CreatedAt)
                .HasColumnType("datetime(3)")
                .HasDefaultValueSql("UTC_TIMESTAMP(3)");

            entity
                .HasIndex(e => e.LeaderboardsId)
                .HasDatabaseName("IX_LeaderboardScores_LeaderboardsId");

            entity
                .HasIndex(e => new { e.DiscordUserId, e.LeaderboardsId })
                .IsUnique()
                .HasDatabaseName("LeaderboardScores_index_2");

            entity
                .HasOne(e => e.Leaderboard)
                .WithMany(l => l.LeaderboardScores)
                .HasForeignKey(e => e.LeaderboardsId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_leaderboard_scores_leaderboards");
        }

        private static void ConfigureLeaderBoardScoreSource(
            EntityTypeBuilder<LeaderboardScoreSourceEntity> entity
        )
        {
            entity.ToTable("LeaderboardScoreSources");

            entity.HasKey(e => e.Id);
            entity
                .Property(e => e.Id)
                .HasColumnName("LeaderboardScoreSourceId")
                .HasColumnType("int unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.LeaderboardsId).HasColumnType("int unsigned").IsRequired();

            entity.Property(e => e.DiscordUserIdActor).HasColumnType("char(19)").IsRequired();

            entity.Property(e => e.DiscordUserIdTarget).HasColumnType("char(19)").IsRequired();

            entity
                .Property(e => e.CreatedAt)
                .HasColumnType("datetime(3)")
                .HasDefaultValueSql("UTC_TIMESTAMP(3)");

            entity
                .HasIndex(e => e.LeaderboardsId)
                .HasDatabaseName("IX_LeaderboardScoreSources_LeaderboardsId");

            entity
                .HasIndex(e => new
                {
                    e.LeaderboardsId,
                    e.DiscordUserIdActor,
                    e.DiscordUserIdTarget,
                })
                .IsUnique()
                .HasDatabaseName("LeaderboardScoreSources_index_2");

            entity
                .HasOne(e => e.Leaderboard)
                .WithMany(l => l.LeaderboardScoreSources)
                .HasForeignKey(e => e.LeaderboardsId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_leaderboard_score_sources_leaderboards");
        }

        private static void ConfigureLeaderboardMessages(
            EntityTypeBuilder<LeaderboardMessagesEntity> entity
        )
        {
            entity.ToTable("LeaderboardMessages");

            entity.HasKey(e => e.Id);
            entity
                .Property(e => e.Id)
                .HasColumnName("LeaderboardMessagesId")
                .HasColumnType("int unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.LeaderboardsId).HasColumnType("int unsigned").IsRequired();

            entity.Property(e => e.DiscordMessageId).HasColumnType("char(19)").IsRequired();

            entity
                .Property(e => e.CreatedAt)
                .HasColumnType("datetime(3)")
                .HasDefaultValueSql("UTC_TIMESTAMP(3)");

            entity
                .HasIndex(e => e.LeaderboardsId)
                .HasDatabaseName("IX_LeaderboardMessages_LeaderboardsId");

            entity
                .HasIndex(e => new { e.LeaderboardsId, e.DiscordMessageId })
                .IsUnique()
                .HasDatabaseName("LeaderboardMessages_index_2");

            entity
                .HasOne(e => e.Leaderboard)
                .WithMany(l => l.LeaderboardMessages)
                .HasForeignKey(e => e.LeaderboardsId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_leaderboard_messages_leaderboards");
        }

        private static void ConfigureWebsiteTraffic(EntityTypeBuilder<WebsiteTrafficEntity> entity)
        {
            entity.ToTable("WebsiteTraffic");

            entity.HasKey(e => e.Id);
            entity
                .Property(e => e.Id)
                .HasColumnName("WebsiteTrafficId")
                .HasColumnType("int unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.HashedIp).HasColumnType("binary(32)").IsRequired();

            entity
                .Property(e => e.ClickedInviteButton)
                .HasColumnType("tinyint(1)")
                .HasDefaultValue(false);

            entity
                .Property(e => e.CreatedAt)
                .HasColumnType("datetime(3)")
                .HasDefaultValueSql("UTC_TIMESTAMP(3)");

            entity.HasIndex(e => e.HashedIp).IsUnique().HasDatabaseName("UniqueIp");
        }

        private static void ConfigureWebsiteTrafficMessages(
            EntityTypeBuilder<WebsiteTrafficMessagesEntity> entity
        )
        {
            entity.ToTable("WebsiteTrafficMessages");

            entity.HasKey(e => e.Id);
            entity
                .Property(e => e.Id)
                .HasColumnName("WebsiteTrafficMessagesId")
                .HasColumnType("int unsigned")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.DiscordMessageId).HasColumnType("char(19)").IsRequired();

            entity
                .Property(e => e.CreatedAt)
                .HasColumnType("datetime(3)")
                .HasDefaultValueSql("UTC_TIMESTAMP(3)");

            entity
                .HasIndex(e => e.DiscordMessageId)
                .IsUnique()
                .HasDatabaseName("WebsiteTrafficMessages_index_1");
        }
    }
}
