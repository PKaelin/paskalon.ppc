// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Infrastructure.PostgreSql.Tests;

namespace paskalON.PowerControls.Infrastructure.IntegrationTest
{
    /// <summary>
    /// Test assembly class for initializing and cleaning up the PostgreSQL container used in integration tests.
    /// </summary>
    [TestClass]
    public static class TestAssembly
    {
        /// <summary>
        /// Initializes the PostgreSQL container before any tests are run.
        /// </summary>
        /// <param name="context">The test context.</param>
        /// <returns>Task.</returns>
        /// <remarks>
        /// A container is spun up for the duration of the test run and is torn down afterward.
        /// This ensures that all tests have a consistent and isolated database environment.
        /// </remarks>
        [AssemblyInitialize]
        public static async Task Initialize(TestContext context)
        {
            await PostgresFixture.StartAsync();
        }


        /// <summary>
        /// Cleans up the PostgreSQL container after all tests have run.
        /// </summary>
        /// <returns>Task.</returns>
        /// <remarks>
        /// Cleaning up the databases is not necessary because the container is stopped and removed
        /// after the tests complete, ensuring no residual data remains.
        /// </remarks>
        [AssemblyCleanup]
        public static async Task Cleanup()
        {
            await PostgresFixture.StopAsync();
        }
    }
}
