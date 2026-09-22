// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.ConstraintEngine.Domain.Configs.Systems;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Strategies;
using paskalON.PowerControls.Domain.Configs.Systems;

namespace paskalON.PowerControls.Infrastructure.IntegrationTest.Storage.SampleData
{
    /// <summary>
    /// Used to create at least one entity of db set.
    /// </summary>
    public class SimpleSetBess
    {
        // System constraints
        public SystemPowerConstraintConfig? SystemPowerConstraintConfig { get; set; }
        public SystemRampConstraintConfig? SystemRampConstraintConfig { get; set; }

        // DER Unit constraints
        public DerUnitPowerConstraintConfig? DerUnitPowerConstraintConfig { get; set; }
        public DerUnitRampConstraintConfig? DerUnitRampConstraintConfig { get; set; }

        // System power control
        public SystemPowerControlConfig? SystemPowerControlConfig { get; set; }

        // DER power control
        public DerUnitPowerControlConfig? DerUnitPowerControlConfig { get; set; }
        public DerUnitEnergyStoragePowerControlConfig? DerUnitEnergyStoragePowerControlConfig { get; set; }


        /// <summary>
        /// Constructor that unusually creates all data.
        /// </summary>
        public SimpleSetBess()
        {
            CreateConstraints();
            CreatePowerControl();
        }

        private void CreateConstraints()
        {
            SystemPowerConstraintConfig = new SystemPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerConstraintConfig",
                MaximumActivePowerWatt = 11111,
                MinimumActivePowerWatt = -11110,
                MaximumReactivePowerVars = 2222,
                MinimumReactivePowerVars = -2220,
                DeratePerUnitInMaintenance = true,
                DeratePerUnitStopped = false,
            };

            SystemRampConstraintConfig = new SystemRampConstraintConfig
            {
                ChangedBy = "Test",
                Name = "SystemRampConstraintConfig",
                MaximumActivePowerWattRampRatePerSecond = 10101,
                MaximumReactivePowerVarsRampRatePerSecond = 202
            };

            DerUnitPowerConstraintConfig = new DerUnitPowerConstraintConfig
            {
                ChangedBy = "Test",
                Name = "DerUnitPowerConstraintConfig",
                MaximumActivePowerWatt = 1111,
                MinimumActivePowerWatt = -1110,
                MaximumReactivePowerVars = 222,
                MinimumReactivePowerVars = -220,
            };

            DerUnitRampConstraintConfig = new DerUnitRampConstraintConfig
            {
                ChangedBy = "Test",
                Name = "DerUnitRampConstraintConfig",
                MaximumActivePowerWattRampRatePerSecond = 1010,
                MaximumReactivePowerVarsRampRatePerSecond = 20
            };
        }


        private void CreatePowerControl()
        {
            SystemPowerControlConfig = new SystemPowerControlConfig
            {
                IsActive = true,
                IsEnabled = true,
                ChangedBy = "Test",
                Name = "SystemPowerControlConfig",
                Constraints = new List<ConstraintBaseConfig> { SystemPowerConstraintConfig!, SystemRampConstraintConfig! }
            };


            DerUnitPowerControlConfig = new DerUnitPowerControlConfig
            {
                IsActive = true,
                IsEnabled = true,
                ChangedBy = "Test",
                Name = "DerUnitPowerControlConfig",
                DerUnitName = "UnitTest",
                DistributionStrategyType = DistributionStrategyType.Priority,
                Constraints = new List<ConstraintBaseConfig> { DerUnitPowerConstraintConfig! }
            };

            DerUnitEnergyStoragePowerControlConfig = new DerUnitEnergyStoragePowerControlConfig
            {
                IsActive = true,
                IsEnabled = true,
                ChangedBy = "Test",
                Name = "DerUnitEnergyStoragePowerControlConfig",
                DerUnitName = "UnitTest",
                DistributionStrategyType = DistributionStrategyType.Weight,
                Constraints = new List<ConstraintBaseConfig> { DerUnitPowerConstraintConfig!, DerUnitRampConstraintConfig! }
            };
        }
    }
}
