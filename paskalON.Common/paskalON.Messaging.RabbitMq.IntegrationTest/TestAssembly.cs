// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Messaging.RabbitMq.Testing;

namespace paskalON.Messaging.RabbitMq.IntegrationTest
{
    /// <summary>
    /// Test assembly class for initializing and cleaning up the RabbitMQ container used in integration tests.
    /// </summary>
    [TestClass]
    public static class TestAssembly
    {
        /// <summary>
        /// Initializes the RabbitMQ container before any tests are run.
        /// </summary>
        /// <param name="context">The test context.</param>
        /// <returns>Task.</returns>
        [AssemblyInitialize]
        public static async Task Initialize(TestContext context)
        {
            await RabbitMqFixture.StartAsync();
        }


        /// <summary>
        /// Cleans up the RabbitMQ container after all tests have run.
        /// </summary>
        /// <returns>Task.</returns>
        [AssemblyCleanup]
        public static async Task Cleanup()
        {
            await RabbitMqFixture.StopAsync();
        }
    }
}
