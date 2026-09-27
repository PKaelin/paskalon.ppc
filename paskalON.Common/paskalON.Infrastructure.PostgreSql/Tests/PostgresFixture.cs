// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Testcontainers.PostgreSql;

namespace paskalON.Infrastructure.PostgreSql.Tests
{
    /// <summary>
    /// A fixture class for managing a PostgreSQL container for integration tests.
    /// </summary>
    public static class PostgresFixture
    {
        /// <summary>
        /// The PostgreSQL container instance used for integration tests.
        /// </summary>
        private static PostgreSqlContainer? _container;

        /// <summary>
        /// Gets the connection string for the PostgreSQL container.
        /// </summary>
        public static string ConnectionString => _container?.GetConnectionString()
            ?? throw new InvalidOperationException("PostgreSQL container has not been started.");


        /// <summary>
        /// Starts the PostgreSQL container for integration tests.
        /// </summary>
        /// <returns>Task.</returns>
        public static async Task StartAsync()
        {
            // Use the same image as in docker compose
            _container = new PostgreSqlBuilder("postgres:18.6")
                .WithDatabase("postgres")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            await _container.StartAsync();
        }


        /// <summary>
        /// Stops and disposes of the PostgreSQL container used for integration tests.
        /// </summary>
        /// <returns>Task.</returns>
        public static async Task StopAsync()
        {
            if (_container is not null)
            {
                await _container.DisposeAsync();
                _container = null;
            }
        }
    }
}
