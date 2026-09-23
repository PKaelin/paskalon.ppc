// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.ConstraintEngine.Domain.Configs.Systems;
using paskalON.Domains.Configs;
using paskalON.PowerControls.Domain.Configs;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Strategies;
using paskalON.PowerControls.Domain.Configs.Systems;
using paskalON.PowerControls.Infrastructure.Storage;

namespace paskalON.DemoSuperSimpleBattery.PowerControls.Data
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
        public static async Task CreateAsync(IPowerControlContext context)
        {
            await CreateCore(context);
            await CreateBasicConstraintsAndControls(context);
        }


        /// <summary>
        /// Create core configuration of the service.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <returns>Task</returns>
        private static async Task CreateCore(IPowerControlContext context)
        {
            SystemConfig systemConfig = new SystemConfig
            {
                ChangedBy = ChangedBy,
                Type = PowerControlType.Bess
            };
            context.SystemConfigs.Add(systemConfig);

            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Create basic constraints and power control configurations.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <returns>Task</returns>
        private static async Task CreateBasicConstraintsAndControls(IPowerControlContext context)
        {
            SystemPowerConstraintConfig systemPowerConstraint = new SystemPowerConstraintConfig
            {
                ChangedBy = ChangedBy,
                Name = "System power constraint",
                MaximumActivePowerWatt = 3630000,
                MinimumActivePowerWatt = -3630000,
                MaximumReactivePowerVars = 3630000,
                MinimumReactivePowerVars = -3630000,
                DeratePerUnitInMaintenance = true,
                DeratePerUnitStopped = true,
            };
            context.SystemPowerConstraintConfigs.Add(systemPowerConstraint);

            DerUnitPowerConstraintConfig derUnitPowerConstraint = new DerUnitPowerConstraintConfig
            {
                ChangedBy = ChangedBy,
                Name = "DER unit constraint",
                MaximumActivePowerWatt = 3630000,
                MinimumActivePowerWatt = -3630000,
                MaximumReactivePowerVars = 3630000,
                MinimumReactivePowerVars = -3630000,
            };
            context.DerUnitPowerConstraintConfigs.Add(derUnitPowerConstraint);


            SystemPowerControlConfig systemPowerControl = new SystemPowerControlConfig
            {
                ChangedBy = ChangedBy,
                Name = "System power control",
                IsActive = true,
                IsEnabled = true,
                Constraints = new List<ConstraintBaseConfig> { systemPowerConstraint }
            };
            context.SystemPowerControlConfigs.Add(systemPowerControl);

            DerUnitEnergyStoragePowerControlConfig derUnitEnergyStorageControl = new DerUnitEnergyStoragePowerControlConfig
            {
                ChangedBy = ChangedBy,
                Name = "DER unit power control - Unit1",
                IsActive = true,
                IsEnabled = true,
                DerUnitName = "BMS-Unit 1",
                DistributionStrategyType = DistributionStrategyType.Proportional,
                Constraints = new List<ConstraintBaseConfig> { derUnitPowerConstraint }
            };
            context.DerUnitEnergyStoragePowerControlConfigs.Add(derUnitEnergyStorageControl);

            await context.SaveChangesAsync();
        }
    }
}
