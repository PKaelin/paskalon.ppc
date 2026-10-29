// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.OpenModes;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.Configs.SafetyFunctions;
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
            await CreateSafetyFunctions(context);
            await CreateBasicOperatingModes(context);
            await CreateLimitingModes(context);
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
                OperatingModeCalculationIntervalMilliseconds = 1000,
                MetricsIntervalMilliseconds = 5000,
                DependencyHeartbeatIntervalMilliseconds = 500,
                RedisLossTimeoutMilliseconds = 1500,
                PowerControlLossTimeoutMilliseconds = 1500,
                DeviceServiceLossTimeoutMilliseconds = 3000,
                ResumeSettleMilliseconds = 2000,
                SetpointRepublishIntervalMilliseconds = 1000,
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
                PublisherTopicCurrentSetpoint = "ppc:om:setpoints",
                PublisherTopicCurrentDerUnitSetpoint = "ppc:om:setpoints:unit",
                PublisherTopicHeartbeat = "ppc:om:heartbeat",
                SubscriberTopicPowerControlHeartbeat = "ppc:pc:heartbeat",
                NameplateMaximumActivePowerWatt = 3630000,
                NameplateMinimumActivePowerWatt = -3630000,
                NameplateMaximumReactivePowerVars = 3630000,
                NameplateMinimumReactivePowerVars = -3630000,
            };
            context.SystemConfigs.Add(systemConfig);

            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Create safety functions  configurations.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <returns>Task</returns>
        private static async Task CreateSafetyFunctions(IOperatingModeContext context)
        {
            RampRateConfig shutDownRampRateConfig = new RampRateConfig
            {
                ChangedBy = ChangedBy,
                RampTimeSeconds = 0,
                RampUpRatePerSecond = 90000,
                RampDownRatePerSecond = 90000,
            };
            context.RampRateConfigs.Add(shutDownRampRateConfig);

            ShutdownModeConfig shutdownMode = new ShutdownModeConfig
            {
                ChangedBy = ChangedBy,
                Name = "ShutdownMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                RampConfig = shutDownRampRateConfig
            };

            context.ShutdownModeConfigs.Add(shutdownMode);

            await context.SaveChangesAsync();
        }



        /// <summary>
        /// Create basic operating modes configurations.
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
                Name = "MaintenanceMode-BMS-Unit1",
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


            ActivePowerModeConfig activePowerModeConfig = new ActivePowerModeConfig
            {
                ChangedBy = ChangedBy,
                Name = "ActivePowerMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                RampConfig = rampRateConfig,
            };
            context.ActivePowerModeConfigs.Add(activePowerModeConfig);

            ReactivePowerModeConfig reactivePowerModeConfig = new ReactivePowerModeConfig
            {
                ChangedBy = ChangedBy,
                Name = "ReactivePowerMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                RampConfig = rampRateConfig,
            };
            context.ReactivePowerModeConfigs.Add(reactivePowerModeConfig);

            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Create limiting operating modes configurations.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <returns>Task</returns>
        private static async Task CreateLimitingModes(IOperatingModeContext context)
        {
            RampRateConfig limitingRampRateConfig = new RampRateConfig
            {
                ChangedBy = ChangedBy,
                RampTimeSeconds = 0,
                RampUpRatePerSecond = 10000,
                RampDownRatePerSecond = 10000,
            };
            context.RampRateConfigs.Add(limitingRampRateConfig);

            ActivePowerLimitModeConfig activePowerLimitModeConfig = new ActivePowerLimitModeConfig
            {
                ChangedBy = ChangedBy,
                Name = "ActivePowerLimitMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                RampConfig = limitingRampRateConfig,
                MaximumActivePowerLimitWatt = 3000000,
                MinimumActivePowerLimitWatt = -3000000,
            };
            context.ActivePowerLimitModeConfigs.Add(activePowerLimitModeConfig);

            ReactivePowerLimitModeConfig reactivePowerLimitModeConfig = new ReactivePowerLimitModeConfig
            {
                ChangedBy = ChangedBy,
                Name = "ReactivePowerLimitMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                RampConfig = limitingRampRateConfig,
                MaximumReactivePowerLimitVars = 2800000,
                MinimumReactivePowerLimitVars = -2800000,
            };
            context.ReactivePowerLimitModeConfigs.Add(reactivePowerLimitModeConfig);

            await context.SaveChangesAsync();
        }
    }
}
