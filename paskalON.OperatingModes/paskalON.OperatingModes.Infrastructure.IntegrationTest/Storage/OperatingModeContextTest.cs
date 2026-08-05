// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Infrastructure.IntegrationTest.Storage.SampleData;
using paskalON.OperatingModes.Infrastructure.Storage;

namespace paskalON.OperatingModes.Infrastructure.IntegrationTest.Storage
{
    [TestClass]
    public class OperatingModeContextTest
    {
        // TODO: Implement more tests to check required and relationships.


        [TestMethod]
        public void CreateDeviceServiceContext()
        {
            using OperatingModeContext context = new OperatingModeContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }


        // TODO: Refine test
        [TestMethod]
        public void CreateOperatingModeTest()
        {
            SimpleOperatingMode sample = new SimpleOperatingMode();

            using (OperatingModeContext context = new OperatingModeContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                // Core
                context.SystemConfigs.Add(sample.SystemConfig!);
                // Ramps
                context.RampRateConfigs.Add(sample.RampRateConfig!);
                context.RampRatePercentageConfigs.Add(sample.RampRatePercentageConfig!);
                context.RampTimeConfigs.Add(sample.RampTimeConfig!);
                context.RampTimeConstantConfigs.Add(sample.RampTimeConstantConfig!);
                // Curves
                context.FrequencyWattCurveConfigs.Add(sample.FrequencyWattCurveConfig!);
                context.CurvePointConfigs.Add(sample.CurvePointConfigFWC1!);
                context.CurvePointConfigs.Add(sample.CurvePointConfigFWC2!);
                context.VoltWattCurveConfigs.Add(sample.VoltWattCurveConfig!);
                context.CurvePointConfigs.Add(sample.CurvePointConfigVWC1!);
                context.VoltVarCurveConfigs.Add(sample.VoltVarCurveConfig!);
                context.CurvePointConfigs.Add(sample.CurvePointConfigVVC1!);
                // Operating closed modes
                context.ActivePowerModeConfigs.Add(sample.ActivePowerModeConfig!);
                context.ReactivePowerModeConfigs.Add(sample.ReactivePowerModeConfig!);
                // Operating open modes
                context.MaintenanceModeConfigs.Add(sample.MaintenanceModeConfig!);
                context.MaximumPowerPointTrackingModeConfigs.Add(sample.MaximumPowerPointTrackingModeConfig!);
                context.ActivePowerFixedModeConfigs.Add(sample.ActivePowerFixedModeConfig!);
                context.ReactivePowerFixedModeConfigs.Add(sample.ReactivePowerFixedModeConfig!);

                context.SaveChanges();
            }

            SystemConfig? systemConfig;
            ActivePowerModeConfig? activePowerModeConfig;

            using (OperatingModeContext context = new OperatingModeContext())
            {
                systemConfig = context.SystemConfigs.FirstOrDefault();
                activePowerModeConfig = context.ActivePowerModeConfigs.Include(x => x.RampConfig).FirstOrDefault();
            }

            Assert.IsNotNull(systemConfig);
            Assert.AreEqual(OperatingModeType.Bess, systemConfig.Type);
            Assert.IsNotNull(activePowerModeConfig);
            Assert.IsNotNull(activePowerModeConfig.RampConfig);
        }
    }
}
