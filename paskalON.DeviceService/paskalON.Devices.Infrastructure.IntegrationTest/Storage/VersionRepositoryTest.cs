// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Devices.Infrastructure.Storage;
using paskalON.Devices.Infrastructure.Storage.Repositories;
using paskalON.Domains;

namespace paskalON.Devices.Infrastructure.IntegrationTest.Storage
{
    [TestClass]
    public class VersionRepositoryTest : DatabaseTestBase
    {
        // DBContext excludes this table because its created via the migration tool hence create it before testing.
        private string _sqlMigrationHistory = @"
                CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
                    ""MigrationId"" character varying(150) NOT NULL,
                    ""ProductVersion"" character varying(32) NOT NULL,
                    CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY (""MigrationId"")
                );";


        [TestMethod]
        public async Task VersionRepositoryNoTableTest()
        {
            await using DeviceServiceContext context = CreateDbContext<DeviceServiceContext>();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            VersionRepository repository = new VersionRepository(NullLogger<VersionRepository>.Instance, context);

            // DBContext excludes this table:
            // modelBuilder.Entity<History>().ToTable(t => t.ExcludeFromMigrations(true));
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await repository.GetDatabaseVersionAsync());
        }


        [TestMethod]
        public async Task VersionRepositoryEmptyTableTest()
        {
            await using DeviceServiceContext context = CreateDbContext<DeviceServiceContext>();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            await context.Database.ExecuteSqlRawAsync(_sqlMigrationHistory);
            VersionRepository repository = new VersionRepository(NullLogger<VersionRepository>.Instance, context);

            string version = await repository.GetDatabaseVersionAsync();

            Assert.IsFalse(string.IsNullOrEmpty(version));
            Assert.IsTrue(version.Contains("No version", StringComparison.OrdinalIgnoreCase));
        }


        [TestMethod]
        public async Task VersionRepositoryTwoRowsTest()
        {
            string? version = null;

            await using (DeviceServiceContext context = CreateDbContext<DeviceServiceContext>())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                await context.Database.ExecuteSqlRawAsync(_sqlMigrationHistory);
                context.Histories.Add(new History { MigrationId = "ZZZ_V1", ProductVersion = "PV1" });
                context.Histories.Add(new History { MigrationId = "ZZZ_V2", ProductVersion = "PV2" });
                context.SaveChanges();
            }

            await using (DeviceServiceContext context = CreateDbContext<DeviceServiceContext>())
            {
                VersionRepository repository = new VersionRepository(NullLogger<VersionRepository>.Instance, context);
                version = await repository.GetDatabaseVersionAsync();
            }

            Assert.IsFalse(string.IsNullOrEmpty(version));
            Assert.IsTrue(version.Contains("ZZZ_V2", StringComparison.OrdinalIgnoreCase));
        }
    }
}
