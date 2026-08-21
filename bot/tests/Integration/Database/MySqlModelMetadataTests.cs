using LundBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Tests.Integration.Database;

public sealed class MySqlModelMetadataTests
{
    [Fact]
    internal void Leaderboards_Id_HasIntUnsignedColumnType()
    {
        using var context = MySqlDbContextFactory.Create();
        var property = context.Model
            .FindEntityType(typeof(LeaderboardsEntity))!
            .FindProperty(nameof(LeaderboardsEntity.Id))!;
        Assert.Equal("int unsigned", property.GetColumnType());
    }

    [Fact]
    internal void Leaderboards_DiscordServerId_HasChar19ColumnType()
    {
        using var context = MySqlDbContextFactory.Create();
        var property = context.Model
            .FindEntityType(typeof(LeaderboardsEntity))!
            .FindProperty(nameof(LeaderboardsEntity.DiscordServerId))!;
        Assert.Equal("char(19)", property.GetColumnType());
    }

    [Fact]
    internal void Leaderboards_Title_HasVarchar64ColumnType()
    {
        using var context = MySqlDbContextFactory.Create();
        var property = context.Model
            .FindEntityType(typeof(LeaderboardsEntity))!
            .FindProperty(nameof(LeaderboardsEntity.Title))!;
        Assert.Equal("varchar(64)", property.GetColumnType());
    }

    [Fact]
    internal void Leaderboards_LeaderboardType_HasVarchar32ColumnType()
    {
        using var context = MySqlDbContextFactory.Create();
        var property = context.Model
            .FindEntityType(typeof(LeaderboardsEntity))!
            .FindProperty(nameof(LeaderboardsEntity.LeaderboardType))!;
        Assert.Equal("varchar(32)", property.GetColumnType());
    }

    [Fact]
    internal void Leaderboards_CreatedAt_HasDatetime3ColumnTypeAndUtcTimestampDefault()
    {
        using var context = MySqlDbContextFactory.Create();
        var property = context.Model
            .FindEntityType(typeof(LeaderboardsEntity))!
            .FindProperty(nameof(LeaderboardsEntity.CreatedAt))!;
        Assert.Equal("datetime(3)", property.GetColumnType());
        Assert.Equal("UTC_TIMESTAMP(3)", property.GetDefaultValueSql());
    }

    [Fact]
    internal void WebsiteTraffic_ClickedInviteButton_HasTinyint1ColumnType()
    {
        using var context = MySqlDbContextFactory.Create();
        var property = context.Model
            .FindEntityType(typeof(WebsiteTrafficEntity))!
            .FindProperty(nameof(WebsiteTrafficEntity.ClickedInviteButton))!;
        Assert.Equal("tinyint(1)", property.GetColumnType());
    }

    [Fact]
    internal void WebsiteTraffic_HashedIp_HasBinary32ColumnType()
    {
        using var context = MySqlDbContextFactory.Create();
        var property = context.Model
            .FindEntityType(typeof(WebsiteTrafficEntity))!
            .FindProperty(nameof(WebsiteTrafficEntity.HashedIp))!;
        Assert.Equal("binary(32)", property.GetColumnType());
    }
}
