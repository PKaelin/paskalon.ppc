// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.OpenModes;
using paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.Configs.SafetyFunctions;
using paskalON.OperatingModes.Domain.OpenModes;
using paskalON.OperatingModes.Domain.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.OperatingModes.Domain.SafetyFunctions;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Application.UnitTest
{
    /// <summary>
    /// Realistic operating modes of a 10 MW / 5 MVar battery plant shared by the application unit tests.
    /// </summary>
    internal static class ApplicationTestData
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
                NameplateMinimumReactivePowerVars = -5000000,
                NameplateMaximumReactivePowerVars = 5000000,
            };
        }


        /// <summary>
        /// Creates a ramp configuration ramping within 60 seconds.
        /// </summary>
        /// <returns>The ramp configuration.</returns>
        public static RampTimeConfig CreateRampConfig()
        {
            return new RampTimeConfig { ChangedBy = "Test", RampUpTimeSeconds = 60, RampDownTimeSeconds = 60 };
        }


        /// <summary>
        /// Creates an active power fixed mode.
        /// </summary>
        /// <returns>The active power fixed mode.</returns>
        public static ActivePowerFixedMode CreateActivePowerFixedMode()
        {
            ActivePowerFixedModeConfig config = new ActivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerFixedMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                RampConfig = CreateRampConfig()
            };
            ActivePowerFixedModeMap map = new ActivePowerFixedModeMap { AvailableActivePower = () => new ActivePower(8000000), AvailableReactivePower = () => new ReactivePower(4000000) };

            return new ActivePowerFixedMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, CreateSystemConfig(), config, map, CreateRampController());
        }


        /// <summary>
        /// Creates a reactive power fixed mode.
        /// </summary>
        /// <returns>The reactive power fixed mode.</returns>
        public static ReactivePowerFixedMode CreateReactivePowerFixedMode()
        {
            ReactivePowerFixedModeConfig config = new ReactivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ReactivePowerFixedMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                RampConfig = CreateRampConfig()
            };
            ReactivePowerFixedModeMap map = new ReactivePowerFixedModeMap { AvailableActivePower = () => new ActivePower(8000000), AvailableReactivePower = () => new ReactivePower(4000000) };

            return new ReactivePowerFixedMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, CreateSystemConfig(), config, map, CreateRampController());
        }


        /// <summary>
        /// Creates a maximum power point tracking mode.
        /// </summary>
        /// <returns>The maximum power point tracking mode.</returns>
        public static MaximumPowerPointTrackingMode CreateMaximumPowerPointTrackingMode()
        {
            MaximumPowerPointTrackingModeConfig config = new MaximumPowerPointTrackingModeConfig
            {
                ChangedBy = "Test",
                Name = "MaximumPowerPointTrackingMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                RampConfig = CreateRampConfig()
            };
            MaximumPowerPointTrackingModeMap map = new MaximumPowerPointTrackingModeMap { AvailableActivePower = () => new ActivePower(8000000), AvailableReactivePower = () => null };

            return new MaximumPowerPointTrackingMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, CreateSystemConfig(), config, map, CreateRampController());
        }


        /// <summary>
        /// Creates a maximum active power limit mode limiting the active power to 6 MW.
        /// </summary>
        /// <returns>The maximum active power limit mode.</returns>
        public static MaximumActivePowerLimitMode CreateMaximumActivePowerLimitMode()
        {
            MaximumActivePowerLimitModeConfig config = new MaximumActivePowerLimitModeConfig
            {
                ChangedBy = "Test",
                Name = "MaximumActivePowerLimitMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                MaximumActivePowerLimitWatts = 6000000,
                RampConfig = CreateRampConfig()
            };
            MaximumActivePowerLimitModeMap map = new MaximumActivePowerLimitModeMap
            {
                AvailableActivePower = () => new ActivePower(8000000),
                AvailableReactivePower = () => null,
                ActivePowerAtPoi = () => null
            };

            return new MaximumActivePowerLimitMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, CreateSystemConfig(), config, map, CreateRampController());
        }


        /// <summary>
        /// Creates a maintenance mode for a DER unit.
        /// </summary>
        /// <param name="name">The name of the maintenance mode.</param>
        /// <param name="derUnitName">The name of the DER unit.</param>
        /// <returns>The maintenance mode.</returns>
        public static MaintenanceMode CreateMaintenanceMode(string name, string derUnitName)
        {
            MaintenanceModeConfig config = new MaintenanceModeConfig
            {
                ChangedBy = "Test",
                Name = name,
                IsActive = true,
                Type = PowerControlType.Bess,
                DerUnitName = derUnitName,
                RampConfig = CreateRampConfig()
            };
            MaintenanceModeMap map = new MaintenanceModeMap
            {
                AvailableActivePower = () => new ActivePower(2000000),
                AvailableReactivePower = () => new ReactivePower(1000000),
                DerUnitName = derUnitName
            };

            return new MaintenanceMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, CreateSystemConfig(), config, map, CreateRampController());
        }


        /// <summary>
        /// Creates a plant-wide shutdown mode.
        /// </summary>
        /// <returns>The shutdown mode.</returns>
        public static ShutdownMode CreateShutdownMode()
        {
            ShutdownModeConfig config = new ShutdownModeConfig
            {
                ChangedBy = "Test",
                Name = "ShutdownMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                RampConfig = CreateRampConfig()
            };
            OperatingModeBaseMap map = new OperatingModeBaseMap { AvailableActivePower = () => null, AvailableReactivePower = () => null };

            return new ShutdownMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, CreateSystemConfig(), config, map, CreateRampController(), null);
        }


        /// <summary>
        /// Creates a dictionary of operating modes keyed by their names.
        /// </summary>
        /// <param name="operatingModes">The operating modes.</param>
        /// <returns>The dictionary of operating modes.</returns>
        public static Dictionary<string, OperatingModeBase> ToDictionary(params OperatingModeBase[] operatingModes)
        {
            return operatingModes.ToDictionary(operatingMode => operatingMode.Name, StringComparer.Ordinal);
        }


        /// <summary>
        /// Creates a mocked ramp controller.
        /// </summary>
        /// <returns>The mocked ramp controller.</returns>
        private static IRampController CreateRampController()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.ShallowCopy()).Returns(new Mock<IRampController>().Object);

            return rampController.Object;
        }
    }
}
