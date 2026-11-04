// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.ClientData.Common;
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.ConstraintEngine.Domain.Configs.Systems;
using paskalON.Domains.Configs;
using paskalON.Infrastructure.PostgreSql.Tests;
using paskalON.PowerControls.Domain.Configs;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Strategies;
using paskalON.PowerControls.Domain.Configs.Systems;
using paskalON.PowerControls.Infrastructure.Storage;

namespace paskalON.PowerControls.Data.IntegrationTest
{
    /// <summary>
    /// Tests of the class <see cref="ClientDataRunner"/> with the power control database.
    /// </summary>
    [TestClass]
    public class ClientDataRunnerTest : PostgresTestBase<PowerControlContext>
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

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<PowerControlContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using PowerControlContext context = CreateDbContext<PowerControlContext>();
            SystemConfig systemConfig = await context.SystemConfigs.SingleAsync();
            Assert.AreEqual(ClientDataLoader.ChangedBy, systemConfig.ChangedBy);
            Assert.AreEqual(1, await context.SystemPowerControlConfigs.CountAsync());
        }


        [TestMethod]
        [DataRow("DemoBattery", PowerControlType.Bess, 25, 0, 90750000d)]
        [DataRow("DemoSimpleBattery", PowerControlType.Bess, 3, 0, 10890000d)]
        [DataRow("DemoSuperSimpleBattery", PowerControlType.Bess, 1, 0, 3630000d)]
        [DataRow("DemoSolar", PowerControlType.Solar, 0, 25, 90750000d)]
        [DataRow("DemoSuperSimpleSolar", PowerControlType.Solar, 0, 1, 3630000d)]
        public async Task RunAsyncImportsDemoClientDataMatchingDevicesTest(string clientProjectName, PowerControlType expectedType, int expectedStorageControls, int expectedUnitControls, double expectedSystemPowerWatt)
        {
            string[] args = [clientProjectName, Path.Combine(_demoDirectory, $"{clientProjectName}.csv")];

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<PowerControlContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using PowerControlContext context = CreateDbContext<PowerControlContext>();
            SystemConfig systemConfig = await context.SystemConfigs.SingleAsync();
            Assert.AreEqual(expectedType, systemConfig.Type);
            SystemPowerConstraintConfig systemConstraint = await context.SystemPowerConstraintConfigs.SingleAsync();
            Assert.AreEqual(expectedSystemPowerWatt, systemConstraint.MaximumActivePowerWatt);
            Assert.AreEqual(-expectedSystemPowerWatt, systemConstraint.MinimumActivePowerWatt);
            Assert.AreEqual(expectedSystemPowerWatt, systemConstraint.MaximumReactivePowerVars);
            Assert.AreEqual(-expectedSystemPowerWatt, systemConstraint.MinimumReactivePowerVars);
            DerUnitPowerConstraintConfig unitConstraint = await context.DerUnitPowerConstraintConfigs.SingleAsync();
            Assert.AreEqual(3630000d, unitConstraint.MaximumActivePowerWatt);
            Assert.AreEqual(expectedStorageControls, await context.DerUnitEnergyStoragePowerControlConfigs.CountAsync());
            Assert.AreEqual(expectedUnitControls, await context.DerUnitPowerControlConfigs.CountAsync());
        }


        [TestMethod]
        public async Task RunAsyncImportsDemoSuperSimpleBatteryTest()
        {
            string[] args = [ClientProjectName, _clientProjectFile];

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<PowerControlContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using PowerControlContext context = CreateDbContext<PowerControlContext>();
            SystemConfig systemConfig = await context.SystemConfigs.SingleAsync();
            Assert.AreEqual(PowerControlType.Bess, systemConfig.Type);
            Assert.AreEqual("ppc:pc:targets", systemConfig.PublisherTopicPowerTarget);
            Assert.AreEqual(1, await context.Configurations.CountAsync());
            SystemPowerControlConfig systemControl = await context.SystemPowerControlConfigs.Include(x => x.Constraints).SingleAsync();
            ConstraintBaseConfig systemConstraint = systemControl.Constraints.Single();
            Assert.IsInstanceOfType<SystemPowerConstraintConfig>(systemConstraint);
            Assert.AreEqual("System power constraint", systemConstraint.Name);
            DerUnitEnergyStoragePowerControlConfig unitControl = await context.DerUnitEnergyStoragePowerControlConfigs.Include(x => x.Constraints).SingleAsync();
            Assert.AreEqual("BMS-Unit 1", unitControl.DerUnitName);
            Assert.AreEqual(DistributionStrategyType.Proportional, unitControl.DistributionStrategyType);
            Assert.IsInstanceOfType<DerUnitPowerConstraintConfig>(unitControl.Constraints.Single());
        }
    }
}
