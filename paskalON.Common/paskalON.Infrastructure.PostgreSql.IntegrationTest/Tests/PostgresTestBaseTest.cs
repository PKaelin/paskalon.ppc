// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Npgsql;
using paskalON.Infrastructure.PostgreSql.IntegrationTest.Tests.SampleData;
using paskalON.Infrastructure.PostgreSql.Tests;

namespace paskalON.Infrastructure.PostgreSql.IntegrationTest.Tests
{
    /// <remarks>
    /// The test class itself derives from <see cref="PostgresTestBase{T}"/> to verify the MSTest
    /// <see cref="TestInitializeAttribute"/> integration. Scenarios that need several databases
    /// use <see cref="PostgresTestDatabase{T}"/> to drive the initialization explicitly.
    /// </remarks>
    [TestClass]
    public class PostgresTestBaseTest : PostgresTestBase<SampleContext>
    {
        private readonly string[] ExpectedMigrations = new string[]
        {
            "20260101000000_InitialCreate",
            "20260102000000_AddUniqueDeviceName"
        };


        private readonly SampleDevice[] SeedDevices = new SampleDevice[]
        {
            new SampleDevice { Id = Guid.Parse("4b1c9a52-1f1e-4a39-9a3c-0c1f2a0b7d11"), Name = "Inverter West", RatedPowerInKilowatt = 250.0 },
            new SampleDevice { Id = Guid.Parse("8e2f6d31-7c4b-4f0e-b5a2-9d3e1c6a4f22"), Name = "Battery Bank North", RatedPowerInKilowatt = 500.0 }
        };


        [TestMethod]
        public async Task InitializeDatabaseCreatesUniquelyNamedDatabaseTest()
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(ConnectionString);

            bool exists = await DatabaseExistsAsync(builder.Database!);

            Assert.MatchesRegex("^test_[0-9a-f]{32}$", builder.Database);
            Assert.IsTrue(exists);
        }


        [TestMethod]
        public void InitializeDatabaseKeepsServerSettingsOfFixtureTest()
        {
            NpgsqlConnectionStringBuilder fixture = new NpgsqlConnectionStringBuilder(PostgresFixture.ConnectionString);
            NpgsqlConnectionStringBuilder testDatabase = new NpgsqlConnectionStringBuilder(ConnectionString);

            Assert.AreEqual(fixture.Host, testDatabase.Host);
            Assert.AreEqual(fixture.Port, testDatabase.Port);
            Assert.AreEqual(fixture.Username, testDatabase.Username);
            Assert.AreEqual(fixture.Password, testDatabase.Password);
            Assert.AreNotEqual(fixture.Database, testDatabase.Database);
        }


        [TestMethod]
        public async Task InitializeDatabaseAppliesAllMigrationsTest()
        {
            await using SampleContext context = CreateDbContext<SampleContext>();

            List<string> applied = (await context.Database.GetAppliedMigrationsAsync()).ToList();
            List<string> pending = (await context.Database.GetPendingMigrationsAsync()).ToList();

            CollectionAssert.AreEqual(ExpectedMigrations, applied);
            Assert.IsEmpty(pending);
        }


        [TestMethod]
        public async Task InitializeDatabaseInvokesSeedAsyncTest()
        {
            await using SampleContext context = CreateDbContext<SampleContext>();

            List<SampleDevice> devices = await context.SampleDevices.AsNoTracking().OrderBy(x => x.Name).ToListAsync();

            Assert.HasCount(2, devices);
            Assert.AreEqual(SeedDevices[1].Id, devices[0].Id);
            Assert.AreEqual("Battery Bank North", devices[0].Name);
            Assert.AreEqual(500.0, devices[0].RatedPowerInKilowatt);
            Assert.AreEqual(SeedDevices[0].Id, devices[1].Id);
            Assert.AreEqual("Inverter West", devices[1].Name);
            Assert.AreEqual(250.0, devices[1].RatedPowerInKilowatt);
        }


        [TestMethod]
        public async Task InitializeDatabaseAppliesUniqueIndexMigrationTest()
        {
            await using SampleContext context = CreateDbContext<SampleContext>();
            context.SampleDevices.Add(new SampleDevice { Id = Guid.NewGuid(), Name = "Inverter West", RatedPowerInKilowatt = 125.0 });

            DbUpdateException exception = await Assert.ThrowsExactlyAsync<DbUpdateException>(() => context.SaveChangesAsync());

            PostgresException postgresException = Assert.IsInstanceOfType<PostgresException>(exception.InnerException);
            Assert.AreEqual(PostgresErrorCodes.UniqueViolation, postgresException.SqlState);
            Assert.AreEqual("IX_SampleDevices_Name", postgresException.ConstraintName);
        }


        [TestMethod]
        [DataRow(1)]
        [DataRow(SampleDevice.NameMaxLength)]
        public async Task InitializeDatabaseAcceptsNameWithinMaximumLengthTest(int nameLength)
        {
            string name = new string('A', nameLength);

            await using (SampleContext context = CreateDbContext<SampleContext>())
            {
                context.SampleDevices.Add(new SampleDevice { Id = Guid.NewGuid(), Name = name, RatedPowerInKilowatt = 75.5 });
                await context.SaveChangesAsync();
            }

            int count;

            await using (SampleContext context = CreateDbContext<SampleContext>())
            {
                count = await context.SampleDevices.CountAsync(x => x.Name == name);
            }

            Assert.AreEqual(1, count);
        }


        [TestMethod]
        public async Task InitializeDatabaseRejectsNameAboveMaximumLengthTest()
        {
            await using SampleContext context = CreateDbContext<SampleContext>();
            context.SampleDevices.Add(new SampleDevice { Id = Guid.NewGuid(), Name = new string('A', SampleDevice.NameMaxLength + 1), RatedPowerInKilowatt = 75.5 });

            DbUpdateException exception = await Assert.ThrowsExactlyAsync<DbUpdateException>(() => context.SaveChangesAsync());

            PostgresException postgresException = Assert.IsInstanceOfType<PostgresException>(exception.InnerException);
            Assert.AreEqual(PostgresErrorCodes.StringDataRightTruncation, postgresException.SqlState);
        }


        [TestMethod]
        public async Task CreateDbContextTargetsTemporaryDatabaseTest()
        {
            await using SampleContext first = CreateDbContext<SampleContext>();
            await using SampleContext second = CreateDbContext<SampleContext>();

            Assert.AreNotSame(first, second);
            NpgsqlConnectionStringBuilder expected = new NpgsqlConnectionStringBuilder(ConnectionString);
            NpgsqlConnectionStringBuilder actual = new NpgsqlConnectionStringBuilder(first.Database.GetConnectionString());
            Assert.AreEqual(expected.Database, actual.Database);
        }


        [TestMethod]
        public async Task CreateDbContextPersistsDataAcrossContextsTest()
        {
            SampleDevice device = new SampleDevice { Id = Guid.NewGuid(), Name = "PV Array South", RatedPowerInKilowatt = 1250.75 };

            await using (SampleContext writer = CreateDbContext<SampleContext>())
            {
                writer.SampleDevices.Add(device);
                await writer.SaveChangesAsync();
            }

            SampleDevice? loaded;

            await using (SampleContext reader = CreateDbContext<SampleContext>())
            {
                loaded = await reader.SampleDevices.AsNoTracking().SingleOrDefaultAsync(x => x.Id == device.Id);
            }

            Assert.IsNotNull(loaded);
            Assert.AreEqual("PV Array South", loaded.Name);
            Assert.AreEqual(1250.75, loaded.RatedPowerInKilowatt);
        }


        [TestMethod]
        public void CreateDbContextWithoutOptionsConstructorThrowsTest()
        {
            Action createContext = () => CreateDbContext<ParameterlessContext>();

            Assert.ThrowsExactly<MissingMethodException>(createContext);
        }


        [TestMethod]
        public async Task InitializeDatabaseIsolatesDataBetweenInstancesTest()
        {
            PostgresTestDatabase<SampleContext> first = new PostgresTestDatabase<SampleContext>();
            PostgresTestDatabase<SampleContext> second = new PostgresTestDatabase<SampleContext>();
            await first.InitializeDatabase();
            await second.InitializeDatabase();

            await using (SampleContext context = first.CreateContext())
            {
                context.SampleDevices.Add(new SampleDevice { Id = Guid.NewGuid(), Name = "Isolated Inverter", RatedPowerInKilowatt = 42.0 });
                await context.SaveChangesAsync();
            }

            int firstCount;
            int secondCount;

            await using (SampleContext context = first.CreateContext())
            {
                firstCount = await context.SampleDevices.CountAsync();
            }

            await using (SampleContext context = second.CreateContext())
            {
                secondCount = await context.SampleDevices.CountAsync();
            }

            Assert.AreNotEqual(first.DatabaseConnectionString, second.DatabaseConnectionString);
            Assert.AreEqual(1, firstCount);
            Assert.AreEqual(0, secondCount);
        }


        [TestMethod]
        public async Task InitializeDatabaseConcurrentlyCreatesDistinctMigratedDatabasesTest()
        {
            const int databaseCount = 8;
            List<PostgresTestDatabase<SampleContext>> databases = Enumerable.Range(0, databaseCount)
                .Select(_ => new PostgresTestDatabase<SampleContext>())
                .ToList();

            await Task.WhenAll(databases.Select(x => x.InitializeDatabase()));

            HashSet<string> names = databases
                .Select(x => new NpgsqlConnectionStringBuilder(x.DatabaseConnectionString).Database!)
                .ToHashSet();
            Assert.HasCount(databaseCount, names);
            foreach (PostgresTestDatabase<SampleContext> database in databases)
            {
                await using SampleContext context = database.CreateContext();
                List<string> applied = (await context.Database.GetAppliedMigrationsAsync()).ToList();
                CollectionAssert.AreEqual(ExpectedMigrations, applied);
            }
        }


        [TestMethod]
        public async Task InitializeDatabaseWhenSeedFailsPropagatesExceptionTest()
        {
            InvalidOperationException seedFailure = new InvalidOperationException("Seeding the sample devices failed.");
            PostgresTestDatabase<SampleContext> database = new PostgresTestDatabase<SampleContext>(_ => throw seedFailure);

            InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => database.InitializeDatabase());

            Assert.AreSame(seedFailure, exception);
        }


        [TestMethod]
        public async Task InitializeDatabaseWithoutMigrationsCreatesNoSchemaTest()
        {
            PostgresTestDatabase<UnmigratedContext> database = new PostgresTestDatabase<UnmigratedContext>();

            await database.InitializeDatabase();

            await using UnmigratedContext context = database.CreateContext();
            PostgresException exception = await Assert.ThrowsExactlyAsync<PostgresException>(() => context.SampleDevices.CountAsync());
            Assert.AreEqual(PostgresErrorCodes.UndefinedTable, exception.SqlState);
        }


        /// <inheritdoc/>
        protected override async Task SeedAsync(SampleContext db)
        {
            foreach (SampleDevice device in SeedDevices)
            {
                db.SampleDevices.Add(new SampleDevice { Id = device.Id, Name = device.Name, RatedPowerInKilowatt = device.RatedPowerInKilowatt });
            }

            await db.SaveChangesAsync();
        }


        private async Task<bool> DatabaseExistsAsync(string databaseName)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(PostgresFixture.ConnectionString)
            {
                Database = "postgres"
            };

            await using NpgsqlConnection connection = new NpgsqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            await using NpgsqlCommand command = new NpgsqlCommand("SELECT EXISTS (SELECT 1 FROM pg_database WHERE datname = @name)", connection);
            command.Parameters.AddWithValue("name", databaseName);
            object? result = await command.ExecuteScalarAsync();

            return result is true;
        }
    }
}
