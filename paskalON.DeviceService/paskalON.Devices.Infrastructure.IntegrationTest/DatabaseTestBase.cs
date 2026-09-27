// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Npgsql;
using paskalON.Devices.Infrastructure.Storage;

namespace paskalON.Devices.Infrastructure.IntegrationTest
{
    /// <summary>
    /// Base class for database integration tests that sets up a temporary PostgreSQL database for each test.
    /// </summary>
    public abstract class DatabaseTestBase
    {
        /// <summary>
        /// The name of the temporary database created for the test.
        /// It is generated uniquely for each test run to ensure isolation.
        /// </summary>
        private string _databaseName = null!;


        /// <summary>
        /// Gets the connection string for the temporary database created for the test.
        /// </summary>
        protected string ConnectionString { get; private set; } = null!;


        /// <summary>
        /// Initializes the temporary database for the test.
        /// </summary>
        [TestInitialize]
        public async Task InitializeDatabase()
        {
            _databaseName = $"test_{Guid.NewGuid():N}";

            await CreateDatabase(_databaseName);

            NpgsqlConnectionStringBuilder connectionString = new NpgsqlConnectionStringBuilder(PostgresFixture.ConnectionString)
            {
                Database = _databaseName
            };

            ConnectionString = connectionString.ConnectionString;
            await using var db = CreateDbContext();
            await db.Database.MigrateAsync();
            await SeedAsync(db);
        }


        /// <summary>
        /// Create a new instance of the DbContext using the temporary database connection string.
        /// </summary>
        /// <returns>A new instance of <see cref="DeviceServiceContext"/>.</returns>
        protected DeviceServiceContext CreateDbContext()
        {
            DbContextOptions<DeviceServiceContext> options = new DbContextOptionsBuilder<DeviceServiceContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            return new DeviceServiceContext(options);
        }


        /// <summary>
        /// Seeds the database with initial data.
        /// Override this method in derived classes to provide custom seeding logic.
        /// </summary>
        /// <param name="db">The <see cref="DeviceServiceContext"/> instance to seed.</param>
        /// <returns>Task.</returns>
        protected virtual Task SeedAsync(DeviceServiceContext db)
        {
            return Task.CompletedTask;
        }


        /// <summary>
        /// Creates a new PostgreSQL database with the specified name.
        /// </summary>
        /// <param name="databaseName">The name of the database to create.</param>
        /// <returns>Task.</returns>
        private static async Task CreateDatabase(string databaseName)
        {
            var connectionString = new NpgsqlConnectionStringBuilder(
                PostgresFixture.ConnectionString)
            {
                Database = "postgres"
            };

            await using var connection =
                new NpgsqlConnection(connectionString.ConnectionString);

            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE \"{databaseName}\"";
            await command.ExecuteNonQueryAsync();
        }
    }
}
