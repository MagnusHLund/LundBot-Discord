using LundBot.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Tests.Fixtures.Data
{
    public sealed class SqliteDbFixture : IDisposable
    {
        public LundBotDiscordDbContext Db { get; private set; }

        private readonly SqliteConnection _connection;

        public SqliteDbFixture()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<LundBotDiscordDbContext>()
                .UseSqlite(_connection)
                .Options;

            Db = new LundBotDiscordDbContext(options);
            Db.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Db.Dispose();
            _connection.Dispose();
        }
    }
}
