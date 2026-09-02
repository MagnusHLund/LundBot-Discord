using LundBot.Domain.Leaderboards;
using LundBot.Domain.MemberJoin;
using LundBot.Domain.WebsiteTraffic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LundBot.Infrastructure.Persistence
{
    public sealed class LundBotDbContext : DbContext
    {
        public DbSet<Leaderboard> Leaderboards { get; set; } = null!;
        public DbSet<LeaderboardScore> LeaderboardScores { get; set; } = null!;
        public DbSet<LeaderboardScoreSource> LeaderboardScoreSources { get; set; } = null!;
        public DbSet<LeaderboardMessage> LeaderboardMessages { get; set; } = null!;
        public DbSet<WebsiteTraffic> WebsiteTraffic { get; set; } = null!;
        public DbSet<WebsiteTrafficMessage> WebsiteTrafficMessages { get; set; } = null!;
        public DbSet<MemberJoinMessage> MemberJoinMessages { get; set; } = null!;

        public LundBotDbContext() { }

        public LundBotDbContext(DbContextOptions<LundBotDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            bool isMySql = Database.ProviderName?.Contains("MySql", StringComparison.OrdinalIgnoreCase) ?? false;

            ConfigureLeaderboards(modelBuilder.Entity<Leaderboard>(), isMySql);
            ConfigureLeaderboardScores(modelBuilder.Entity<LeaderboardScore>(), isMySql);
            ConfigureLeaderboardScoreSource(modelBuilder.Entity<LeaderboardScoreSource>(), isMySql);
            ConfigureLeaderboardMessages(modelBuilder.Entity<LeaderboardMessage>(), isMySql);
            ConfigureWebsiteTraffic(modelBuilder.Entity<WebsiteTraffic>(), isMySql);
            ConfigureWebsiteTrafficMessages(modelBuilder.Entity<WebsiteTrafficMessage>(), isMySql);
            ConfigureMemberJoinMessages(modelBuilder.Entity<MemberJoinMessage>(), isMySql);
        }

        private static void ConfigureLeaderboards(EntityTypeBuilder<Leaderboard> entity, bool isMySql)
        {
            entity.ToTable("Leaderboards");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity.Property(e => e.Id).HasColumnName("LeaderboardsId").ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(entity.Property(e => e.DiscordServerId).IsRequired(), isMySql, "bigint unsigned");
            ConfigureMySqlColumnType(entity.Property(e => e.DiscordChannelId).IsRequired(), isMySql, "bigint unsigned");
            ConfigureMySqlColumnType(entity.Property(e => e.Title).IsRequired(), isMySql, "varchar(64)");
            ConfigureMySqlColumnType(entity.Property(e => e.Message).IsRequired(), isMySql, "varchar(256)");

            ConfigureMySqlColumnType(
                entity.Property(e => e.LeaderboardType).HasConversion<string>().IsRequired(),
                isMySql,
                "varchar(32)"
            );

            ConfigureMySqlColumnType(
                entity.Property(e => e.CreatedAt).HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

            entity
                .HasIndex(nameof(Leaderboard.DiscordServerId), nameof(Leaderboard.DiscordChannelId))
                .IsUnique()
                .HasDatabaseName("leaderboards_index_2");

            entity
                .HasIndex(nameof(Leaderboard.DiscordServerId), nameof(Leaderboard.LeaderboardType))
                .HasDatabaseName("leaderboards_index_3");

            entity.HasIndex(e => e.LeaderboardType);
        }

        private static void ConfigureLeaderboardScores(EntityTypeBuilder<LeaderboardScore> entity, bool isMySql)
        {
            entity.ToTable("LeaderboardScores");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity.Property(e => e.Id).HasColumnName("LeaderboardScoreId").ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(entity.Property(e => e.LeaderboardId).IsRequired(), isMySql, "int unsigned");
            ConfigureMySqlColumnType(entity.Property(e => e.DiscordUserId).IsRequired(), isMySql, "bigint unsigned");
            ConfigureMySqlColumnType(entity.Property(e => e.Score).IsRequired(), isMySql, "int unsigned");

            ConfigureMySqlColumnType(
                entity
                    .Property(e => e.UpdatedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

            ConfigureMySqlColumnType(
                entity.Property(e => e.CreatedAt).HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

            entity.HasIndex(e => e.LeaderboardId).HasDatabaseName("IX_LeaderboardScore_LeaderboardsId");

            entity
                .HasIndex(e => new { e.DiscordUserId, e.LeaderboardId })
                .IsUnique()
                .HasDatabaseName("LeaderboardScore_index_2");

            entity
                .HasOne(e => e.Leaderboard)
                .WithMany(l => l.LeaderboardScores)
                .HasForeignKey(e => e.LeaderboardId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_leaderboard_scores_leaderboards");
        }

        private static void ConfigureLeaderboardScoreSource(
            EntityTypeBuilder<LeaderboardScoreSource> entity,
            bool isMySql
        )
        {
            entity.ToTable("LeaderboardScoreSource");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity.Property(e => e.Id).HasColumnName("LeaderboardScoreSourceId").ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(entity.Property(e => e.LeaderboardId).IsRequired(), isMySql, "int unsigned");
            ConfigureMySqlColumnType(
                entity.Property(e => e.DiscordUserIdActor).IsRequired(),
                isMySql,
                "bigint unsigned"
            );
            ConfigureMySqlColumnType(
                entity.Property(e => e.DiscordUserIdTarget).IsRequired(),
                isMySql,
                "bigint unsigned"
            );

            ConfigureMySqlColumnType(
                entity.Property(e => e.CreatedAt).HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

            entity.HasIndex(e => e.LeaderboardId).HasDatabaseName("IX_LeaderboardScoreSource_LeaderboardsId");

            entity
                .HasIndex(e => new
                {
                    e.LeaderboardId,
                    e.DiscordUserIdActor,
                    e.DiscordUserIdTarget,
                })
                .HasDatabaseName("LeaderboardScoreSource_index_2");

            entity
                .HasOne(e => e.Leaderboard)
                .WithMany(l => l.LeaderboardScoreSources)
                .HasForeignKey(e => e.LeaderboardId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_leaderboard_score_source_leaderboards");
        }

        private static void ConfigureLeaderboardMessages(EntityTypeBuilder<LeaderboardMessage> entity, bool isMySql)
        {
            entity.ToTable("LeaderboardMessages");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity.Property(e => e.Id).HasColumnName("LeaderboardMessagesId").ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(entity.Property(e => e.LeaderboardId).IsRequired(), isMySql, "int unsigned");
            ConfigureMySqlColumnType(entity.Property(e => e.DiscordMessageId).IsRequired(), isMySql, "bigint unsigned");

            ConfigureMySqlColumnType(
                entity.Property(e => e.CreatedAt).HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

            entity.HasIndex(e => e.LeaderboardId).HasDatabaseName("IX_LeaderboardMessages_LeaderboardsId");

            entity
                .HasIndex(e => new { e.LeaderboardId, e.DiscordMessageId })
                .IsUnique()
                .HasDatabaseName("LeaderboardMessages_index_2");

            entity
                .HasOne(e => e.Leaderboard)
                .WithMany(l => l.LeaderboardMessages)
                .HasForeignKey(e => e.LeaderboardId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_leaderboard_messages_leaderboards");
        }

        private static void ConfigureWebsiteTraffic(EntityTypeBuilder<WebsiteTraffic> entity, bool isMySql)
        {
            entity.ToTable("WebsiteTraffic");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity.Property(e => e.Id).HasColumnName("WebsiteTrafficId").ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(entity.Property(e => e.HashedIp).IsRequired(), isMySql, "binary(32)");
            ConfigureMySqlColumnType(
                entity.Property(e => e.ClickedInviteButton).HasDefaultValue(false),
                isMySql,
                "tinyint(1)"
            );

            ConfigureMySqlColumnType(
                entity.Property(e => e.CreatedAt).HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

            entity.HasIndex(e => e.HashedIp).IsUnique().HasDatabaseName("UniqueIp");
        }

        private static void ConfigureWebsiteTrafficMessages(
            EntityTypeBuilder<WebsiteTrafficMessage> entity,
            bool isMySql
        )
        {
            entity.ToTable("WebsiteTrafficMessages");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity.Property(e => e.Id).HasColumnName("WebsiteTrafficMessageId").ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(entity.Property(e => e.DiscordMessageId).IsRequired(), isMySql, "bigint unsigned");

            ConfigureMySqlColumnType(
                entity.Property(e => e.CreatedAt).HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

            entity.HasIndex(e => e.DiscordMessageId).IsUnique().HasDatabaseName("WebsiteTrafficMessage_index_1");
        }

        private static void ConfigureMemberJoinMessages(EntityTypeBuilder<MemberJoinMessage> entity, bool isMySql)
        {
            entity.ToTable("MemberJoinMessages");

            entity.HasKey(e => e.Id);
            ConfigureMySqlColumnType(
                entity.Property(e => e.Id).HasColumnName("MemberJoinMessageId").ValueGeneratedOnAdd(),
                isMySql,
                "int unsigned"
            );

            ConfigureMySqlColumnType(entity.Property(e => e.DiscordMessageId).IsRequired(), isMySql, "bigint unsigned");
            ConfigureMySqlColumnType(entity.Property(e => e.DiscordUserId).IsRequired(), isMySql, "bigint unsigned");

            ConfigureMySqlColumnType(
                entity.Property(e => e.CreatedAt).HasDefaultValueSql(GetCreatedAtDefaultSql(isMySql)),
                isMySql,
                "datetime(3)"
            );

            entity.HasIndex(e => e.DiscordUserId).IsUnique().HasDatabaseName("MemberJoinMessages_index_1");
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
