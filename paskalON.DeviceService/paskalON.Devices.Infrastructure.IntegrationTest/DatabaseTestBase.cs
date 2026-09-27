// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Infrastructure.Storage;
using paskalON.Infrastructure.PostgreSql.Tests;

namespace paskalON.Devices.Infrastructure.IntegrationTest
{
    /// <summary>
    /// Base class for database integration tests that sets up a temporary PostgreSQL database for each test.
    /// </summary>
    public abstract class DatabaseTestBase : PostgresTestBase<DeviceServiceContext>
    {
    }
}
