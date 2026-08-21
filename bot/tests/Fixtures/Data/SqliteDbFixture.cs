using LundBot.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Tests.Fixtures.Data
{
    internal sealed class SqliteDbFixture : IDisposable
    {
        internal LundBotDiscordDbContext Db { get; private set; }

        private readonly SqliteConnection _connection;

        internal SqliteDbFixture()
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
