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

        private static readonly string _clientProjectFile = Path.Combine(AppContext.BaseDirectory, "ClientData", "Demo", "DemoSuperSimpleBattery.csv");


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
