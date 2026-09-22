// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.ConstraintEngine.Domain.Configs.Systems;
using paskalON.Domains;
using paskalON.PowerControls.Domain.Configs;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Systems;

namespace paskalON.PowerControls.Infrastructure.Storage
{
    public interface IPowerControlContext
    {
        // Core DbSet
        DbSet<Configuration> Configurations { get; set; }            // General configuration class for the microservice
        DbSet<History> Histories { get; set; }                       // For DB migration history.
        DbSet<SystemConfig> SystemConfigs { get; set; }

        //-----------------------------------------------------------------------------------------
        // Constraints
        //-----------------------------------------------------------------------------------------
        // System constraints
        DbSet<SystemPowerConstraintConfig> SystemPowerConstraintConfigs { get; set; }
        DbSet<SystemRampConstraintConfig> SystemRampConstraintConfigs { get; set; }

        // DER constraints
        DbSet<DerUnitPowerConstraintConfig> DerUnitPowerConstraintConfigs { get; set; }
        DbSet<DerUnitRampConstraintConfig> DerUnitRampConstraintConfig { get; set; }


        //-----------------------------------------------------------------------------------------
        // Power Control
        //-----------------------------------------------------------------------------------------
        // System power control
        DbSet<SystemPowerControlConfig> SystemPowerControlConfigs { get; set; }

        // DER power control
        DbSet<DerUnitPowerControlConfig> DerUnitPowerControlConfigs { get; set; }
        DbSet<DerUnitEnergyStoragePowerControlConfig> DerUnitEnergyStoragePowerControlConfigs { get; set; }


        /// <summary>
        /// Save changes.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task result contains the number of state entries written to the underlying database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
