// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Infrastructure.IntegrationTest.Storage.SampleData;
using paskalON.OperatingModes.Infrastructure.Storage;

namespace paskalON.OperatingModes.Infrastructure.IntegrationTest.Storage
{
    [TestClass]
    public class OperatingModeContextTest : DatabaseTestBase
    {
        [TestMethod]
        public async Task CreateOperatingModeContext()
        {
            await using OperatingModeContext context = CreateDbContext<OperatingModeContext>();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }


        // TODO: Refine test
        [TestMethod]
        public async Task CreateOperatingModeTest()
        {
            SimpleOperatingMode sample = new SimpleOperatingMode();

            await using (OperatingModeContext context = CreateDbContext<OperatingModeContext>())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
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

                await context.SaveChangesAsync();
            }

            SystemConfig? systemConfig;
            ActivePowerModeConfig? activePowerModeConfig;

            await using (OperatingModeContext context = CreateDbContext<OperatingModeContext>())
            {
                systemConfig = await context.SystemConfigs.FirstOrDefaultAsync();
                activePowerModeConfig = await context.ActivePowerModeConfigs.Include(x => x.RampConfig).FirstOrDefaultAsync();
            }

            Assert.IsNotNull(systemConfig);
            Assert.AreEqual(OperatingModeType.Bess, systemConfig.Type);
            Assert.IsNotNull(activePowerModeConfig);
            Assert.IsNotNull(activePowerModeConfig.RampConfig);
        }
    }
}
