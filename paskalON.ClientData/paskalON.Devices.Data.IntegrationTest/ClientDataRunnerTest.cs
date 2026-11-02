// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.ClientData.Common;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Infrastructure.Storage;
using paskalON.Domains.Configs;
using paskalON.Infrastructure.PostgreSql.Tests;

namespace paskalON.Devices.Data.IntegrationTest
{
    /// <summary>
    /// Tests of the class <see cref="ClientDataRunner"/> with the device service database.
    /// </summary>
    [TestClass]
    public class ClientDataRunnerTest : PostgresTestBase<DeviceServiceContext>
    {
        private const string ClientProjectName = "DemoSuperSimpleBattery";

        private static readonly string _clientProjectFile = Path.Combine(AppContext.BaseDirectory, "ClientData", "Demo", "DemoSuperSimpleBattery.csv");


        [TestMethod]
        public async Task RunAsyncImportsDemoSuperSimpleBatteryTest()
        {
            string[] args = [ClientProjectName, _clientProjectFile];

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<DeviceServiceContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using DeviceServiceContext context = CreateDbContext<DeviceServiceContext>();
            SystemConfig systemConfig = await context.SystemConfigs.SingleAsync();
            Assert.AreEqual(PowerControlType.Bess, systemConfig.Type);
            Assert.AreEqual("ppc:pc:targets", systemConfig.SubscriberTopicPowerTarget);
            Assert.AreEqual(ClientDataLoader.ChangedBy, systemConfig.ChangedBy);
            Configuration configuration = await context.Configurations.SingleAsync();
            Assert.AreEqual("AuditCleanUpInMonths", configuration.Key);
            Assert.AreEqual("60", configuration.Value);
            PowerConversionSystemConfig pcs = await context.PowerConversionSystemConfigs.Include(x => x.ModbusConfig).Include(x => x.DerUnitConfig).SingleAsync();
            Assert.AreEqual((ushort)4400, pcs.ModbusConfig.Port);
            Assert.AreEqual("BMS-Unit 1", pcs.DerUnitConfig.Name);
            BatteryBankConfig batteryBank = await context.BatteryBankConfigs.Include(x => x.BatteryBankDeviceConfig).Include(x => x.ModbusConfig).SingleAsync();
            Assert.AreEqual(11, batteryBank.DeviceId);
            Assert.AreEqual((ushort)4600, batteryBank.ModbusConfig.Port);
            Assert.AreEqual(BatteryType.LithiumIon, batteryBank.BatteryBankDeviceConfig.BatteryType);
            SystemPowerMeterConfig meter = await context.SystemPowerMeterConfigs.Include(x => x.DerConfig).Include(x => x.C37Config).Include(x => x.PowerMeterDeviceConfig).ThenInclude(x => x.PowerMeterMapC37Config).SingleAsync();
            Assert.AreEqual("Der 1", meter.DerConfig.Name);
            Assert.IsNotNull(meter.C37Config);
            Assert.AreEqual((ushort)4800, meter.C37Config.Port);
            Assert.AreEqual("Analog0", meter.PowerMeterDeviceConfig.PowerMeterMapC37Config!.ApparentPower);
            Assert.AreEqual(0, await context.BatteryBankDeviceCustomConfigs.CountAsync());
        }


        [TestMethod]
        public async Task RunAsyncMigratesToSelectedSchemaVersionTest()
        {
            string[] args = [ClientProjectName, _clientProjectFile, "0.1"];

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<DeviceServiceContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using DeviceServiceContext context = CreateDbContext<DeviceServiceContext>();
            Assert.AreEqual(1, await context.DerConfigs.CountAsync());
        }


        [TestMethod]
        public async Task RunAsyncKeepsDatabaseWhenSchemaVersionIsUnknownTest()
        {
            await AddExistingDerAsync();
            string[] args = [ClientProjectName, _clientProjectFile, "9.9"];

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<DeviceServiceContext>);

            Assert.AreEqual((int)ClientDataExitCode.InvalidArguments, exitCode);
            await AssertExistingDerIsKeptAsync();
        }


        [TestMethod]
        public async Task RunAsyncKeepsDatabaseWhenFileIsInvalidTest()
        {
            await AddExistingDerAsync();
            string file = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.csv");
            await File.WriteAllLinesAsync(file, ["Type=DerGroupConfig,Name=Group 1,DerConfig=Missing"]);
            string[] args = [ClientProjectName, file];

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<DeviceServiceContext>);
            File.Delete(file);

            Assert.AreEqual((int)ClientDataExitCode.CsvValidationError, exitCode);
            await AssertExistingDerIsKeptAsync();
        }


        [TestMethod]
        public async Task RunAsyncReturnsInvalidArgumentsWhenFileDoesNotExistTest()
        {
            string[] args = [ClientProjectName, "DoesNotExist.csv"];

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<DeviceServiceContext>);

            Assert.AreEqual((int)ClientDataExitCode.InvalidArguments, exitCode);
        }


        [TestMethod]
        public async Task RunAsyncReturnsDatabaseErrorWhenDatabaseIsNotReachableTest()
        {
            DbContextOptions<DeviceServiceContext> options = new DbContextOptionsBuilder<DeviceServiceContext>()
                .UseNpgsql("Host=localhost:1;Username=admin;Password=admin;Database=devices;Timeout=1")
                .Options;
            string[] args = [ClientProjectName, _clientProjectFile];

            int exitCode = await ClientDataRunner.RunAsync(args, () => new DeviceServiceContext(options));

            Assert.AreEqual((int)ClientDataExitCode.DatabaseError, exitCode);
        }


        /// <summary>
        /// Adds an entity that must still exist if the import did not start.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task AddExistingDerAsync()
        {
            await using DeviceServiceContext context = CreateDbContext<DeviceServiceContext>();
            context.DerConfigs.Add(new DerConfig { ChangedBy = "Test", Name = "Existing" });
            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Asserts that the database has not been recreated.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task AssertExistingDerIsKeptAsync()
        {
            await using DeviceServiceContext context = CreateDbContext<DeviceServiceContext>();
            DerConfig der = await context.DerConfigs.SingleAsync();
            Assert.AreEqual("Existing", der.Name);
        }
    }
}
