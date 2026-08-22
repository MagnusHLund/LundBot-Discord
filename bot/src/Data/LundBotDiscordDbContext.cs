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
        public DbSet<WelcomeMessageEntity> WelcomeMessages { get; set; } = null!;

        public LundBotDiscordDbContext() { }

        public LundBotDiscordDbContext(DbContextOptions<LundBotDiscordDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            bool isMySql =
                Database.ProviderName?.Contains("MySql", StringComparison.OrdinalIgnoreCase)
                ?? false;

            ConfigureLeaderboards(modelBuilder.Entity<LeaderboardsEntity>(), isMySql);
            ConfigureLeaderboardScores(modelBuilder.Entity<LeaderboardScoresEntity>(), isMySql);
            ConfigureLeaderBoardScoreSource(
                modelBuilder.Entity<LeaderboardScoreSourceEntity>(),
                isMySql
            );
            ConfigureLeaderboardMessages(modelBuilder.Entity<LeaderboardMessagesEntity>(), isMySql);
            ConfigureWebsiteTraffic(modelBuilder.Entity<WebsiteTrafficEntity>(), isMySql);
            ConfigureWebsiteTrafficMessages(
                modelBuilder.Entity<WebsiteTrafficMessagesEntity>(),
                isMySql
            );
            ConfigureWelcomeMessages(modelBuilder.Entity<WelcomeMessageEntity>(), isMySql);
        }

        private static void ConfigureLeaderboards(
            EntityTypeBuilder<LeaderboardsEntity> entity,
            bool isMySql
        )
        {
            entity.ToTable("Leaderboards");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity.Property(e => e.Id).HasColumnName("LeaderboardsId").ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(
                entity.Property(e => e.DiscordServerId).IsRequired(),
                isMySql,
                "char(19)"
            );
            ConfigureMySqlColumnType(
                entity.Property(e => e.DiscordChannelId).IsRequired(),
                isMySql,
                "char(19)"
            );
            ConfigureMySqlColumnType(
                entity.Property(e => e.Title).IsRequired(),
                isMySql,
                "varchar(64)"
            );
            ConfigureMySqlColumnType(
                entity.Property(e => e.Message).IsRequired(),
                isMySql,
                "varchar(256)"
            );

            ConfigureMySqlColumnType(
                entity.Property(e => e.LeaderboardType).HasConversion<string>().IsRequired(),
                isMySql,
                "varchar(32)"
            );

            ConfigureMySqlColumnType(
                entity
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

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
            EntityTypeBuilder<LeaderboardScoresEntity> entity,
            bool isMySql
        )
        {
            entity.ToTable("LeaderboardScores");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity
                    .Property(e => e.Id)
                    .HasColumnName("LeaderboardScoresId")
                    .ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(
                entity.Property(e => e.LeaderboardsId).IsRequired(),
                isMySql,
                "int unsigned"
            );
            ConfigureMySqlColumnType(
                entity.Property(e => e.DiscordUserId).IsRequired(),
                isMySql,
                "char(19)"
            );
            ConfigureMySqlColumnType(
                entity.Property(e => e.Score).IsRequired(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(
                entity
                    .Property(e => e.UpdatedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

            ConfigureMySqlColumnType(
                entity
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

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
            EntityTypeBuilder<LeaderboardScoreSourceEntity> entity,
            bool isMySql
        )
        {
            entity.ToTable("LeaderboardScoreSources");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity
                    .Property(e => e.Id)
                    .HasColumnName("LeaderboardScoreSourceId")
                    .ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(
                entity.Property(e => e.LeaderboardsId).IsRequired(),
                isMySql,
                "int unsigned"
            );
            ConfigureMySqlColumnType(
                entity.Property(e => e.DiscordUserIdActor).IsRequired(),
                isMySql,
                "char(19)"
            );
            ConfigureMySqlColumnType(
                entity.Property(e => e.DiscordUserIdTarget).IsRequired(),
                isMySql,
                "char(19)"
            );

            ConfigureMySqlColumnType(
                entity
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

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
            EntityTypeBuilder<LeaderboardMessagesEntity> entity,
            bool isMySql
        )
        {
            entity.ToTable("LeaderboardMessages");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity
                    .Property(e => e.Id)
                    .HasColumnName("LeaderboardMessagesId")
                    .ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(
                entity.Property(e => e.LeaderboardsId).IsRequired(),
                isMySql,
                "int unsigned"
            );
            ConfigureMySqlColumnType(
                entity.Property(e => e.DiscordMessageId).IsRequired(),
                isMySql,
                "char(19)"
            );

            ConfigureMySqlColumnType(
                entity
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

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

        private static void ConfigureWebsiteTraffic(
            EntityTypeBuilder<WebsiteTrafficEntity> entity,
            bool isMySql
        )
        {
            entity.ToTable("WebsiteTraffic");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity.Property(e => e.Id).HasColumnName("WebsiteTrafficId").ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(
                entity.Property(e => e.HashedIp).IsRequired(),
                isMySql,
                "binary(32)"
            );
            ConfigureMySqlColumnType(
                entity.Property(e => e.ClickedInviteButton).HasDefaultValue(false),
                isMySql,
                "tinyint(1)"
            );

            ConfigureMySqlColumnType(
                entity
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

            entity.HasIndex(e => e.HashedIp).IsUnique().HasDatabaseName("UniqueIp");
        }

        private static void ConfigureWebsiteTrafficMessages(
            EntityTypeBuilder<WebsiteTrafficMessagesEntity> entity,
            bool isMySql
        )
        {
            entity.ToTable("WebsiteTrafficMessages");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity
                    .Property(e => e.Id)
                    .HasColumnName("WebsiteTrafficMessagesId")
                    .ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(
                entity.Property(e => e.DiscordMessageId).IsRequired(),
                isMySql,
                "char(19)"
            );

            ConfigureMySqlColumnType(
                entity
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

            entity
                .HasIndex(e => e.DiscordMessageId)
                .IsUnique()
                .HasDatabaseName("WebsiteTrafficMessages_index_1");
        }

        private static void ConfigureWelcomeMessages(
            EntityTypeBuilder<WelcomeMessageEntity> entity,
            bool isMySql
        )
        {
            entity.ToTable("WelcomeMessages");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity.Property(e => e.Id).HasColumnName("WelcomeMessagesId").ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(
                entity.Property(e => e.DiscordMessageId).IsRequired(),
                isMySql,
                "char(19)"
            );
            ConfigureMySqlColumnType(
                entity.Property(e => e.DiscordUserId).IsRequired(),
                isMySql,
                "char(19)"
            );

            ConfigureMySqlColumnType(
                entity
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

            entity
                .HasIndex(e => e.DiscordUserId)
                .IsUnique()
                .HasDatabaseName("WelcomeMessages_index_1");
        }

        private static string GetCreatedAtDefaultSql(bool isMySql) =>
            isMySql ? "UTC_TIMESTAMP(3)" : "CURRENT_TIMESTAMP";

        private static PropertyBuilder<TProperty> ConfigureMySqlColumnType<TProperty>(
            PropertyBuilder<TProperty> property,
            bool isMySql,
            string mySqlColumnType
        )
        {
            // Apply MySQL/MariaDB-specific column types only when using the Pomelo provider.
            // Tests use SQLite, which has different type mappings and does not support these
            // provider-specific declarations consistently.

            if (isMySql)
            {
                property.HasColumnType(mySqlColumnType);
            }

            return property;
        }
    }
}
