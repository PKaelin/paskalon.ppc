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
        /// <param name="image">The Docker image to use for the PostgreSQL container. Default is "postgres:18.6".</param>
        /// <param name="database">The name of the database to create in the PostgreSQL container. Default is "postgres".</param>
        /// <param name="username">The username for the PostgreSQL container. Default is "postgres".</param>
        /// <param name="password">The password for the PostgreSQL container. Default is "postgres".</param>
        /// <returns>Task.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a container has already been started. Call <see cref="StopAsync"/> first.
        /// </exception>
        /// <remarks>
        /// The container is only registered after it started successfully.
        /// If the start fails, the container is disposed and the fixture stays stopped.
        /// </remarks>
        public static async Task StartAsync(string image = "postgres:18.6", string database = "postgres", string username = "postgres", string password = "postgres")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(image);
            ArgumentException.ThrowIfNullOrWhiteSpace(database);
            ArgumentException.ThrowIfNullOrWhiteSpace(username);
            ArgumentException.ThrowIfNullOrWhiteSpace(password);

            if (_container is not null)
            {
                throw new InvalidOperationException("PostgreSQL container has already been started. Call StopAsync before starting a new container.");
            }

            // Use the same image as in docker compose
            PostgreSqlContainer container = new PostgreSqlBuilder(image)
                .WithDatabase(database)
                .WithUsername(username)
                .WithPassword(password)
                .Build();

            try
            {
                await container.StartAsync();
            }
            catch
            {
                await container.DisposeAsync();
                throw;
            }

            _container = container;
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
