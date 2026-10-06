// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Npgsql;
using paskalON.Infrastructure.PostgreSql.Tests;
using System.Data;

namespace paskalON.Infrastructure.PostgreSql.IntegrationTest.Tests
{
    /// <remarks>
    /// Tests that stop the shared container are marked with <see cref="DoNotParallelizeAttribute"/>
    /// so they run in isolation, and always restore a running default container afterwards so
    /// they do not depend on execution order.
    /// </remarks>
    [TestClass]
    public class PostgresFixtureTest
    {
        private const string DefaultImage = "postgres:18.6";
        private const string DefaultImageVersion = "18.6";
        private const string UnknownImage = "paskalon/postgres-does-not-exist:0.0.0";
        private const string NotStartedMessage = "PostgreSQL container has not been started.";
        private const string AlreadyStartedMessage = "PostgreSQL container has already been started. Call StopAsync before starting a new container.";


        [TestMethod]
        public void ConnectionStringUsesDefaultSettingsTest()
        {
            string connectionString = PostgresFixture.ConnectionString;

            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(connectionString);
            Assert.AreEqual("postgres", builder.Database);
            Assert.AreEqual("postgres", builder.Username);
            Assert.AreEqual("postgres", builder.Password);
            Assert.IsFalse(string.IsNullOrWhiteSpace(builder.Host));
            Assert.IsGreaterThan(0, builder.Port);
        }


        [TestMethod]
        public async Task ConnectionStringConnectsToExpectedServerVersionTest()
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(PostgresFixture.ConnectionString);
            await connection.OpenAsync();

            await using NpgsqlCommand command = new NpgsqlCommand("SHOW server_version", connection);

            object? serverVersion = await command.ExecuteScalarAsync();

            Assert.IsInstanceOfType<string>(serverVersion);
            Assert.StartsWith(DefaultImageVersion, (string)serverVersion);
        }


        [TestMethod]
        [DataRow(null, "postgres", "postgres", "postgres")]
        [DataRow("", "postgres", "postgres", "postgres")]
        [DataRow("   ", "postgres", "postgres", "postgres")]
        [DataRow(DefaultImage, null, "postgres", "postgres")]
        [DataRow(DefaultImage, "", "postgres", "postgres")]
        [DataRow(DefaultImage, "postgres", null, "postgres")]
        [DataRow(DefaultImage, "postgres", " ", "postgres")]
        [DataRow(DefaultImage, "postgres", "postgres", null)]
        [DataRow(DefaultImage, "postgres", "postgres", "")]
        public async Task StartAsyncWithInvalidArgumentThrowsAndKeepsRunningContainerTest(string? image, string? database, string? username, string? password)
        {
            string runningConnectionString = PostgresFixture.ConnectionString;

            await Assert.ThrowsAsync<ArgumentException>(() => PostgresFixture.StartAsync(image!, database!, username!, password!));

            Assert.AreEqual(runningConnectionString, PostgresFixture.ConnectionString);

            await using NpgsqlConnection connection = new NpgsqlConnection(PostgresFixture.ConnectionString);
            await connection.OpenAsync();
            Assert.AreEqual(ConnectionState.Open, connection.State);
        }


        [TestMethod]
        [DoNotParallelize]
        public async Task StopAsyncMakesConnectionStringUnavailableTest()
        {
            try
            {
                await PostgresFixture.StopAsync();

                InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() => PostgresFixture.ConnectionString);

                Assert.AreEqual(NotStartedMessage, exception.Message);
            }
            finally
            {
                await RestoreDefaultContainerAsync();
            }
        }


        [TestMethod]
        [DoNotParallelize]
        public async Task StopAsyncAllowsStartingNewContainerTest()
        {
            string previousConnectionString = PostgresFixture.ConnectionString;

            try
            {
                await PostgresFixture.StopAsync();
                await PostgresFixture.StartAsync();

                Assert.AreNotEqual(previousConnectionString, PostgresFixture.ConnectionString);

                await using NpgsqlConnection connection = new NpgsqlConnection(PostgresFixture.ConnectionString);
                await connection.OpenAsync();
                Assert.AreEqual(ConnectionState.Open, connection.State);
            }
            finally
            {
                await RestoreDefaultContainerAsync();
            }
        }


        [TestMethod]
        [DoNotParallelize]
        public async Task StartAsyncWithCustomSettingsCreatesMatchingServerTest()
        {
            const string database = "plant_control";
            const string username = "plant_operator";
            const string password = "S3cure!Passw0rd";

            try
            {
                await PostgresFixture.StopAsync();
                await PostgresFixture.StartAsync(DefaultImage, database, username, password);

                NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(PostgresFixture.ConnectionString);
                Assert.AreEqual(database, builder.Database);
                Assert.AreEqual(username, builder.Username);
                Assert.AreEqual(password, builder.Password);

                await using NpgsqlConnection connection = new NpgsqlConnection(PostgresFixture.ConnectionString);
                await connection.OpenAsync();

                await using NpgsqlCommand command = new NpgsqlCommand("SELECT current_database() || '|' || current_user", connection);
                object? identity = await command.ExecuteScalarAsync();
                Assert.AreEqual($"{database}|{username}", identity);
            }
            finally
            {
                await RestoreDefaultContainerAsync();
            }
        }


        [TestMethod]
        [DataRow(DefaultImage)]
        [DataRow(UnknownImage)]
        public async Task StartAsyncWhenAlreadyStartedThrowsAndKeepsRunningContainerTest(string image)
        {
            string runningConnectionString = PostgresFixture.ConnectionString;

            InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => PostgresFixture.StartAsync(image));

            Assert.AreEqual(AlreadyStartedMessage, exception.Message);
            Assert.AreEqual(runningConnectionString, PostgresFixture.ConnectionString);

            await using NpgsqlConnection connection = new NpgsqlConnection(PostgresFixture.ConnectionString);
            await connection.OpenAsync();
            Assert.AreEqual(ConnectionState.Open, connection.State);
        }


        [TestMethod]
        [DoNotParallelize]
        public async Task StartAsyncWithUnknownImageWhenStoppedLeavesFixtureStoppedTest()
        {
            try
            {
                await PostgresFixture.StopAsync();

                await Assert.ThrowsAsync<Exception>(() => PostgresFixture.StartAsync(UnknownImage));

                InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() => PostgresFixture.ConnectionString);
                Assert.AreEqual(NotStartedMessage, exception.Message);
            }
            finally
            {
                await RestoreDefaultContainerAsync();
            }
        }


        /// <summary>
        /// Restores a running container with default settings for the remaining tests,
        /// independent of the state a test left the fixture in.
        /// </summary>
        /// <returns>Task.</returns>
        private static async Task RestoreDefaultContainerAsync()
        {
            await PostgresFixture.StopAsync();
            await PostgresFixture.StartAsync();
        }
    }
}
