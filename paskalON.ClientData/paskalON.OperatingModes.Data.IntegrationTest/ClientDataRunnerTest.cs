// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.ClientData.Common;
using paskalON.Domains.Configs;
using paskalON.Infrastructure.PostgreSql.Tests;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.OpenModes;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.Configs.SafetyFunctions;
using paskalON.OperatingModes.Infrastructure.Storage;

namespace paskalON.OperatingModes.Data.IntegrationTest
{
    /// <summary>
    /// Tests of the class <see cref="ClientDataRunner"/> with the operating mode database.
    /// </summary>
    [TestClass]
    public class ClientDataRunnerTest : PostgresTestBase<OperatingModeContext>
    {
        private const string ClientProjectName = "DemoSuperSimpleBattery";

        private static readonly string _demoDirectory = Path.Combine(AppContext.BaseDirectory, "ClientData", "Demo");

        private static readonly string _clientProjectFile = Path.Combine(_demoDirectory, "DemoSuperSimpleBattery.csv");


        /// <summary>
        /// Gets the client project names of all demo CSV files in the folder ClientData/Demo.
        /// </summary>
        public static IEnumerable<object[]> DemoClientProjectNames => Directory
            .EnumerateFiles(_demoDirectory, "*.csv")
            .Select(file => new object[] { Path.GetFileNameWithoutExtension(file) })
            .OrderBy(x => (string)x[0], StringComparer.Ordinal);


        [TestMethod]
        [DynamicData(nameof(DemoClientProjectNames))]
        public async Task RunAsyncImportsDemoClientDataTest(string clientProjectName)
        {
            string[] args = [clientProjectName, Path.Combine(_demoDirectory, $"{clientProjectName}.csv")];

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<OperatingModeContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using OperatingModeContext context = CreateDbContext<OperatingModeContext>();
            SystemConfig systemConfig = await context.SystemConfigs.SingleAsync();
            Assert.AreEqual(ClientDataLoader.ChangedBy, systemConfig.ChangedBy);
            Assert.AreEqual(1, await context.ShutdownModeConfigs.CountAsync());
        }


        [TestMethod]
        [DataRow("DemoBattery", PowerControlType.Bess, 25, "BMS-Unit 55", -1d, 1d)]
        [DataRow("DemoSimpleBattery", PowerControlType.Bess, 3, "BMS-Unit 3", -1d, 1d)]
        [DataRow("DemoSuperSimpleBattery", PowerControlType.Bess, 1, "BMS-Unit 1", -1d, 1d)]
        [DataRow("DemoSolar", PowerControlType.Solar, 25, "PV-Unit 55", 0d, 0d)]
        [DataRow("DemoSuperSimpleSolar", PowerControlType.Solar, 1, "PV-Unit 1", 0d, 0d)]
        public async Task RunAsyncImportsDemoClientDataMatchingDevicesTest(string clientProjectName, PowerControlType expectedType, int pcsCount, string lastDerUnitName,
            double minimumActiveFactor, double reactiveFactor)
        {
            string[] args = [clientProjectName, Path.Combine(_demoDirectory, $"{clientProjectName}.csv")];

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<OperatingModeContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using OperatingModeContext context = CreateDbContext<OperatingModeContext>();
            SystemConfig systemConfig = await context.SystemConfigs.SingleAsync();
            Assert.AreEqual(expectedType, systemConfig.Type);
            Assert.AreEqual(3630000d * pcsCount, systemConfig.NameplateMaximumActivePowerWatt);
            Assert.AreEqual(minimumActiveFactor * 3630000d * pcsCount, systemConfig.NameplateMinimumActivePowerWatt);
            Assert.AreEqual(reactiveFactor * 3630000d * pcsCount, systemConfig.NameplateMaximumReactivePowerVars);
            Assert.AreEqual(-reactiveFactor * 3630000d * pcsCount, systemConfig.NameplateMinimumReactivePowerVars);
            List<MaintenanceModeConfig> maintenanceModes = await context.MaintenanceModeConfigs.ToListAsync();
            Assert.AreEqual(pcsCount, maintenanceModes.Count);
            Assert.AreEqual(pcsCount, maintenanceModes.Select(x => x.Name).Distinct().Count());
            Assert.IsTrue(maintenanceModes.Any(x => x.DerUnitName == lastDerUnitName));
            Assert.IsTrue(maintenanceModes.All(x => x.Type == expectedType));
            ActivePowerLimitModeConfig activeLimitMode = await context.ActivePowerLimitModeConfigs.SingleAsync();
            Assert.AreEqual(3000000d * pcsCount, activeLimitMode.MaximumActivePowerLimitWatt);
            Assert.AreEqual(minimumActiveFactor * 3000000d * pcsCount, activeLimitMode.MinimumActivePowerLimitWatt);
            ReactivePowerLimitModeConfig reactiveLimitMode = await context.ReactivePowerLimitModeConfigs.SingleAsync();
            Assert.AreEqual(reactiveFactor * 2800000d * pcsCount, reactiveLimitMode.MaximumReactivePowerLimitVars);
            Assert.AreEqual(-reactiveFactor * 2800000d * pcsCount, reactiveLimitMode.MinimumReactivePowerLimitVars);
        }


        [TestMethod]
        public async Task RunAsyncImportsDemoSuperSimpleBatteryTest()
        {
            string[] args = [ClientProjectName, _clientProjectFile];

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<OperatingModeContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using OperatingModeContext context = CreateDbContext<OperatingModeContext>();
            SystemConfig systemConfig = await context.SystemConfigs.SingleAsync();
            Assert.AreEqual(PowerControlType.Bess, systemConfig.Type);
            Assert.AreEqual(60.0, systemConfig.ReferenceFrequency);
            Assert.AreEqual(1, await context.Configurations.CountAsync());
            Assert.AreEqual(3, await context.RampRateConfigs.CountAsync());
            Assert.AreEqual(8, await context.OperatingModeBaseConfigs.CountAsync());
            ShutdownModeConfig shutdownMode = await context.ShutdownModeConfigs.Include(x => x.RampConfig).SingleAsync();
            Assert.AreEqual(90000, ((RampRateConfig)shutdownMode.RampConfig).RampUpRatePerSecond);
            ActivePowerLimitModeConfig limitMode = await context.ActivePowerLimitModeConfigs.Include(x => x.RampConfig).SingleAsync();
            Assert.AreEqual(10000, ((RampRateConfig)limitMode.RampConfig).RampDownRatePerSecond);
            Assert.AreEqual(3000000, limitMode.MaximumActivePowerLimitWatt);
        }
    }
}
