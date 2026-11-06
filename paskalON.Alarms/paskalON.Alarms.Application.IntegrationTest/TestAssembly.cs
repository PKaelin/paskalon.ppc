// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Infrastructure.PostgreSql.Tests;
using paskalON.Messaging.RabbitMq.Testing;

namespace paskalON.Alarms.Application.IntegrationTest
{
    /// <summary>
    /// Test assembly class for initializing and cleaning up the PostgreSQL and RabbitMQ containers used in integration tests.
    /// </summary>
    [TestClass]
    public static class TestAssembly
    {
        /// <summary>
        /// Initializes the PostgreSQL and RabbitMQ containers before any tests are run.
        /// </summary>
        /// <param name="context">The test context.</param>
        /// <returns>Task.</returns>
        [AssemblyInitialize]
        public static async Task Initialize(TestContext context)
        {
            await Task.WhenAll(PostgresFixture.StartAsync(), RabbitMqFixture.StartAsync());
        }


        /// <summary>
        /// Cleans up the containers after all tests have run.
        /// </summary>
        /// <returns>Task.</returns>
        [AssemblyCleanup]
        public static async Task Cleanup()
        {
            await Task.WhenAll(PostgresFixture.StopAsync(), RabbitMqFixture.StopAsync());
        }
    }
}
