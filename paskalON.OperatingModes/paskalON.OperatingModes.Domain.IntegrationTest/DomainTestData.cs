// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Application.Ramps;
using paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.Configs.SafetyFunctions;
using paskalON.OperatingModes.Domain.IntegrationTest.DummyModes;
using paskalON.OperatingModes.Domain.SafetyFunctions;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.IntegrationTest
{
    internal static class DomainTestData
    {
        /// <summary>
        /// Creates the system configuration of the test plant.
        /// </summary>
        /// <returns>The system configuration.</returns>
        public static SystemConfig CreateSystemConfig()
        {
            return new SystemConfig
            {
                ChangedBy = "Test",
                Type = PowerControlType.Bess,
                ReferenceFrequency = 50,
                NameplateMinimumActivePowerWatt = -10000000,
                NameplateMaximumActivePowerWatt = 10000000,
                NameplateMinimumReactivePowerVars = -10000000,
                NameplateMaximumReactivePowerVars = 10000000,
            };
        }


        /// <summary>
        /// Creates a shutdown mode with the specified time provider.
        /// </summary>
        /// <param name="timeProvider">The time provider to use for the shutdown mode.</param>
        /// <returns>The created shutdown mode.</returns>
        public static ShutdownMode CreateShutdownMode(TimeProvider timeProvider)
        {
            RampRateConfig overrideRamp = new RampRateConfig
            {
                ChangedBy = "Test",
                RampUpRatePerSecond = ActivePower.FromMega(4).Watts,
                RampDownRatePerSecond = ActivePower.FromMega(4).Watts
            };

            ShutdownModeConfig overrideModeConfig = new ShutdownModeConfig
            {
                ChangedBy = "Test",
                IsActive = true,
                Type = PowerControlType.Bess,
                Name = "Shutdown Mode",
                ShutdownTargetActivePowerWatts = 0,
                ShutdownTargetReactivePowerVars = 0,
                RampConfig = overrideRamp
            };

            OperatingModeBaseMap overrideMap = new OperatingModeBaseMap
            {
                AvailableActivePower = () => null,
                AvailableReactivePower = () => null
            };

            ShutdownMode overrideMode = new ShutdownMode(NullLogger.Instance, timeProvider, new Mock<IMetricsPublisher>().Object,
                CreateSystemConfig(), overrideModeConfig, overrideMap, new RampController(NullLogger.Instance, timeProvider, overrideRamp));

            return overrideMode;
        }


        public static ActivePowerLimitMode CreateActivePowerLimitingMode(TimeProvider timeProvider, double limitMegaWatts, Func<ActivePower?> activePowerAtPoiProvider)
        {
            RampRateConfig limitingRamp = new RampRateConfig
            {
                ChangedBy = "Test",
                RampUpRatePerSecond = ActivePower.FromMega(1).Watts,
                RampDownRatePerSecond = ActivePower.FromMega(1).Watts
            };

            ActivePowerLimitModeConfig limitConfig = new ActivePowerLimitModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerLimitMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                MaximumActivePowerLimitWatt = ActivePower.FromMega(limitMegaWatts).Watts,
                RampConfig = limitingRamp
            };

            ActivePowerLimitModeMap limitingMap = new ActivePowerLimitModeMap
            {
                AvailableActivePower = () => ActivePower.FromMega(10),
                AvailableReactivePower = () => null,
                ActivePowerAtPoi = activePowerAtPoiProvider
            };

            ActivePowerLimitMode limitMode = new ActivePowerLimitMode(
                NullLogger.Instance,
                timeProvider,
                new Mock<IMetricsPublisher>().Object,
                CreateSystemConfig(),
                limitConfig,
                limitingMap,
                new RampController(NullLogger.Instance, timeProvider, limitingRamp));

            return limitMode;
        }


        /// <summary>
        /// Creates a dummy additive active mode with a 500 kW per second ramp and 10 MW available active power.
        /// </summary>
        /// <param name="timeProvider">The time provider to use for the dummy additive active mode.</param>
        /// <param name="deltaMegaWatts">The active power delta in MW added to the base active power.</param>
        /// <returns>The created dummy additive active mode.</returns>
        public static DummyAdditiveActiveMode CreateDummyAdditiveActiveMode(TimeProvider timeProvider, double deltaMegaWatts)
        {
            RampRateConfig additiveRamp = new RampRateConfig
            {
                ChangedBy = "Test",
                RampUpRatePerSecond = ActivePower.FromKilo(500).Watts,
                RampDownRatePerSecond = ActivePower.FromKilo(500).Watts
            };

            DummyAdditiveActiveModeConfig additiveConfig = new DummyAdditiveActiveModeConfig
            {
                ChangedBy = "Test",
                Name = "DummyAdditiveActiveMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = 1000,
                DeadbandAvailable = 1000,
                RampConfig = additiveRamp
            };

            OperatingModeBaseMap additiveMap = new OperatingModeBaseMap
            {
                AvailableActivePower = () => ActivePower.FromMega(10),
                AvailableReactivePower = () => null
            };

            return new DummyAdditiveActiveMode(NullLogger.Instance, timeProvider, new Mock<IMetricsPublisher>().Object, CreateSystemConfig(), additiveConfig,
                additiveMap, new RampController(NullLogger.Instance, timeProvider, additiveRamp), ActivePower.FromMega(deltaMegaWatts));
        }
    }
}
