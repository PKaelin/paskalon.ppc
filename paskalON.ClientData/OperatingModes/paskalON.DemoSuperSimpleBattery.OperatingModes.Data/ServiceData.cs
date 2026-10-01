// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Infrastructure.Storage;

namespace paskalON.DemoSuperSimpleBattery.OperatingModes.Data
{
    static class ServiceData
    {
        /// <summary>
        /// Initial changed by user.
        /// </summary>
        private const string ChangedBy = "System Init";


        /// <summary>
        /// Main method to create the service data.
        /// </summary>
        /// <param name="context">DB context interface.</param>
        public static async Task CreateAsync(IOperatingModeContext context)
        {
            await CreateCore(context);
            await CreateBasicOperatingModes(context);
        }


        /// <summary>
        /// Create core configuration of the service.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <returns>Task</returns>
        private static async Task CreateCore(IOperatingModeContext context)
        {
            SystemConfig systemConfig = new SystemConfig
            {
                ChangedBy = ChangedBy,
                Type = PowerControlType.Bess,
                ReferenceFrequency = 60.0,
                MetricsIntervalMilliseconds = 5000,
                SubscriberTopicPcsCore = "ppc:device:pcs:core",
                SubscriberTopicPcsDetail = "ppc:device:pcs:detail",
                SubscriberTopicBatteryBankCore = "ppc:device:bb:core",
                SubscriberTopicBatteryBankDetail = "ppc:device:bb:detail",
                SubscriberTopicSolarPanelCore = "ppc:device:pv:core",
                SubscriberTopicSolarPanelDetail = "ppc:device:pv:detail",
                SubscriberTopicExternalPowerMeterCore = "ppc:device:pm:external:core",
                SubscriberTopicExternalPowerMeterDetail = "ppc:device:pm:external:detail",
                SubscriberTopicAuxiliaryPowerMeterCore = "ppc:device:pm:auxiliary:core",
                SubscriberTopicAuxiliaryPowerMeterDetail = "ppc:device:pm:auxiliary:detail",
                SubscriberTopicSystemPowerMeterCore = "ppc:device:pm:system:core",
                SubscriberTopicSystemPowerMeterDetail = "ppc:device:pm:system:detail",
                SubscriberTopicCircuitPowerMeterCore = "ppc:device:pm:circuit:core",
                SubscriberTopicCircuitPowerMeterDetail = "ppc:device:pm:circuit:detail",
            };
            context.SystemConfigs.Add(systemConfig);

            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Create basic constraints and power control configurations.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <returns>Task</returns>
        private static async Task CreateBasicOperatingModes(IOperatingModeContext context)
        {
            RampRateConfig rampRateConfig = new RampRateConfig
            {
                ChangedBy = ChangedBy,
                RampTimeSeconds = 0,
                RampUpRatePerSecond = 60500,
                RampDownRatePerSecond = 60500,
            };
            context.RampRateConfigs.Add(rampRateConfig);

            MaintenanceModeConfig maintenanceModeConfig = new MaintenanceModeConfig
            {
                ChangedBy = ChangedBy,
                Name = "MaintenanceMode",
                DerUnitName = "BMS-Unit 1",
                IsActive = true,
                Type = PowerControlType.Bess,
                RampConfig = rampRateConfig,
            };
            context.MaintenanceModeConfigs.Add(maintenanceModeConfig);

            ActivePowerFixedModeConfig activePowerFixedModeConfig = new ActivePowerFixedModeConfig
            {
                ChangedBy = ChangedBy,
                Name = "ActivePowerFixedMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                RampConfig = rampRateConfig,
            };
            context.ActivePowerFixedModeConfigs.Add(activePowerFixedModeConfig);

            ReactivePowerFixedModeConfig reactivePowerFixedModeConfig = new ReactivePowerFixedModeConfig
            {
                ChangedBy = ChangedBy,
                Name = "ReactivePowerFixedMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                RampConfig = rampRateConfig,
            };
            context.ReactivePowerFixedModeConfigs.Add(reactivePowerFixedModeConfig);

            await context.SaveChangesAsync();
        }
    }
}
