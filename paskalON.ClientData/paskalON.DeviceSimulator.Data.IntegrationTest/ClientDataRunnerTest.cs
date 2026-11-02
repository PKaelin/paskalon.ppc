// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.ClientData.Common;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Infrastructure.Storage;
using paskalON.Infrastructure.PostgreSql.Tests;

namespace paskalON.DeviceSimulator.Data.IntegrationTest
{
    /// <summary>
    /// Tests of the class <see cref="ClientDataRunner"/> with the device simulator database.
    /// </summary>
    [TestClass]
    public class ClientDataRunnerTest : PostgresTestBase<DeviceServiceContext>
    {
        private const string ClientProjectName = "DemoSuperSimpleBattery";

        private static readonly string _clientProjectFile = Path.Combine(AppContext.BaseDirectory, "ClientData", "Demo", "DemoSuperSimpleBattery.csv");


        [TestMethod]
        public async Task RunAsyncImportsDeviceDataAndSimulatorCustomConfigsTest()
        {
            string[] args = [ClientProjectName, _clientProjectFile];

            int exitCode = await ClientDataRunner.RunAsync(args, CreateDbContext<DeviceServiceContext>);

            Assert.AreEqual((int)ClientDataExitCode.Success, exitCode);
            await using DeviceServiceContext context = CreateDbContext<DeviceServiceContext>();
            Assert.AreEqual(1, await context.SystemPowerMeterConfigs.CountAsync());
            PowerConversionSystemDeviceConfig pcsDevice = await context.PowerConversionSystemDeviceConfigs.Include(x => x.Customs).SingleAsync();
            Assert.AreEqual("PcsSimulationKey", pcsDevice.Customs.Single().Key);
            BatteryBankDeviceConfig batteryBankDevice = await context.BatteryBankDeviceConfigs.Include(x => x.Customs).SingleAsync();
            BatteryBankDeviceCustomConfig preCharge = batteryBankDevice.Customs.Single(x => x.Key == "SimPreChargeDelaySeconds");
            Assert.HasCount(2, batteryBankDevice.Customs);
            Assert.AreEqual("0", preCharge.Value);
            Assert.AreEqual(ClientDataLoader.ChangedBy, preCharge.ChangedBy);
            PowerMeterDeviceConfig powerMeterDevice = await context.PowerMeterDeviceConfigs.Include(x => x.Customs).SingleAsync();
            PowerMeterDeviceCustomConfig voltage = powerMeterDevice.Customs.Single(x => x.Key == "SimGridNominalVoltageLineToLine");
            Assert.HasCount(2, powerMeterDevice.Customs);
            Assert.AreEqual("34500", voltage.Value);
        }
    }
}
