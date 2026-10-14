// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.Controls;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.UnitTest
{
    /// <summary>
    /// Realistic test data shared by the operating mode unit tests.
    /// </summary>
    internal static class OperatingModeTestData
    {
        /// <summary>
        /// Nameplate maximum active power of the test plant in Watts (10 MW).
        /// </summary>
        public const double NameplateActivePowerWatt = 10000000;


        /// <summary>
        /// Nameplate maximum reactive power of the test plant in Vars (5 MVar).
        /// </summary>
        public const double NameplateReactivePowerVars = 5000000;


        /// <summary>
        /// Setpoint and available deadband of the test operating modes in base units (10 kW / 10 kVar).
        /// </summary>
        public const double Deadband = 10000;


        /// <summary>
        /// Creates a system configuration of a 10 MW / 5 MVar battery plant.
        /// </summary>
        /// <returns>The system configuration.</returns>
        public static SystemConfig CreateSystemConfig()
        {
            return new SystemConfig
            {
                ChangedBy = "Test",
                Type = PowerControlType.Bess,
                ReferenceFrequency = 50,
                NameplateMinimumActivePowerWatt = -NameplateActivePowerWatt,
                NameplateMaximumActivePowerWatt = NameplateActivePowerWatt,
                NameplateMinimumReactivePowerVars = -NameplateReactivePowerVars,
                NameplateMaximumReactivePowerVars = NameplateReactivePowerVars,
            };
        }


        /// <summary>
        /// Creates an operating mode configuration with a 60 second ramp.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <returns>The operating mode configuration.</returns>
        public static TestOperatingModeConfig CreateConfig(string name)
        {
            return new TestOperatingModeConfig
            {
                ChangedBy = "Test",
                Name = name,
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = Deadband,
                DeadbandAvailable = Deadband,
                RampConfig = CreateRampConfig(),
            };
        }


        /// <summary>
        /// Creates a ramp configuration ramping up and down within 60 seconds.
        /// </summary>
        /// <returns>The ramp configuration.</returns>
        public static RampTimeConfig CreateRampConfig()
        {
            return new RampTimeConfig
            {
                ChangedBy = "Test",
                RampUpTimeSeconds = 60,
                RampDownTimeSeconds = 60,
            };
        }


        /// <summary>
        /// Creates an operating mode map with 8 MW / 4 MVar available power.
        /// </summary>
        /// <returns>The operating mode map.</returns>
        public static OperatingModeBaseMap CreateMap()
        {
            return new OperatingModeBaseMap
            {
                AvailableActivePower = () => new ActivePower(8000000),
                AvailableReactivePower = () => new ReactivePower(4000000),
            };
        }


        /// <summary>
        /// Creates a mocked ramp controller whose shallow copy is a separate mock.
        /// </summary>
        /// <returns>The mocked ramp controller.</returns>
        public static Mock<IRampController> CreateRampController()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.ShallowCopy()).Returns(new Mock<IRampController>().Object);

            return rampController;
        }


        /// <summary>
        /// Creates power control limits of the test plant for active power.
        /// </summary>
        /// <returns>The power control limits.</returns>
        public static PowerControlLimits CreateActiveLimits()
        {
            return new PowerControlLimits(-NameplateActivePowerWatt, NameplateActivePowerWatt, Deadband, Deadband);
        }


        /// <summary>
        /// Test operating mode configuration.
        /// </summary>
        internal sealed class TestOperatingModeConfig : OperatingModeBaseConfig
        {
        }
    }
}
