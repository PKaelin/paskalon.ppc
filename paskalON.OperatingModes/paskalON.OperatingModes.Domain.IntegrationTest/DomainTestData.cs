// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Application.Ramps;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.Configs.SafetyFunctions;
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
    }
}
