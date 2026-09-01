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
    public class VersionRepositoryTest
    {
        private DbContextOptions<DeviceServiceContext>? _options;


        [TestInitialize]
        public void Initialize()
        {
            string variable = "DB_CONNECTION_STRING";
            string? connectionString = Environment.GetEnvironmentVariable(variable);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString, variable);
            _options = new DbContextOptionsBuilder<DeviceServiceContext>().UseNpgsql(connectionString).Options;
        }


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
            using DeviceServiceContext context = new DeviceServiceContext(_options!);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            VersionRepository repository = new VersionRepository(NullLogger.Instance, context);

            // DBContext excludes this table:
            // modelBuilder.Entity<History>().ToTable(t => t.ExcludeFromMigrations(true));
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await repository.GetDatabaseVersionAsync());
        }


        [TestMethod]
        public async Task VersionRepositoryEmptyTableTest()
        {
            using DeviceServiceContext context = new DeviceServiceContext(_options!);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            await context.Database.ExecuteSqlRawAsync(_sqlMigrationHistory);
            VersionRepository repository = new VersionRepository(NullLogger.Instance, context);

            string version = await repository.GetDatabaseVersionAsync();

            Assert.IsFalse(string.IsNullOrEmpty(version));
            Assert.IsTrue(version.Contains("No version", StringComparison.OrdinalIgnoreCase));
        }


        [TestMethod]
        public async Task VersionRepositoryTwoRowsTest()
        {
            string? version = null;

            using (DeviceServiceContext context = new DeviceServiceContext(_options!))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                await context.Database.ExecuteSqlRawAsync(_sqlMigrationHistory);
                context.Histories.Add(new History { MigrationId = "ZZZ_V1", ProductVersion = "PV1" });
                context.Histories.Add(new History { MigrationId = "ZZZ_V2", ProductVersion = "PV2" });
                context.SaveChanges();
            }

            using (DeviceServiceContext context = new DeviceServiceContext(_options!))
            {
                VersionRepository repository = new VersionRepository(NullLogger.Instance, context);
                version = await repository.GetDatabaseVersionAsync();
            }

            Assert.IsFalse(string.IsNullOrEmpty(version));
            Assert.IsTrue(version.Contains("ZZZ_V2", StringComparison.OrdinalIgnoreCase));
        }
    }
}
