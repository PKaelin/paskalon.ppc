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

        private static readonly string _clientProjectFile = Path.Combine(AppContext.BaseDirectory, "ClientData", "Demo", "DemoSuperSimpleBattery.csv");


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
