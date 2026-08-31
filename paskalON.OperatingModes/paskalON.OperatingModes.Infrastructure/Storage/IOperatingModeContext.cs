// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.Domains;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.EnergyStorages;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageActives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.Curves;
using paskalON.OperatingModes.Domain.Configs.Modes.ComplexPower;
using paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.Ramps;

namespace paskalON.OperatingModes.Infrastructure.Storage
{
    internal interface IOperatingModeContext
    {
        // Core DbSet
        DbSet<Configuration> Configurations { get; set; }            // General configuration class for the microservice
        DbSet<History> Histories { get; set; }                       // For DB migration history.
        DbSet<SystemConfig> SystemConfigs { get; set; }

        // Ramps
        DbSet<RampRateConfig> RampRateConfigs { get; set; }
        DbSet<RampRatePercentageConfig> RampRatePercentageConfigs { get; set; }
        DbSet<RampTimeConfig> RampTimeConfigs { get; set; }
        DbSet<RampTimeConstantConfig> RampTimeConstantConfigs { get; set; }

        // Curves
        DbSet<CurvePointConfig> CurvePointConfigs { get; set; }
        DbSet<VoltWattCurveConfig> VoltWattCurveConfigs { get; set; }
        DbSet<VoltVarCurveConfig> VoltVarCurveConfigs { get; set; }
        DbSet<FrequencyWattCurveConfig> FrequencyWattCurveConfigs { get; set; }

        // Operating closed modes
        DbSet<MaintenanceSocModeConfig> MaintenanceSocModeConfigs { get; set; }
        // Operating closed modes Energy Storages
        DbSet<ChargeDischargeModeConfig> ChargeDischargeModeConfigs { get; set; }
        DbSet<CoordinatedChargeDischargeModeConfig> CoordinatedChargeDischargeModeConfigs { get; set; }
        // Operating closed modes Frequency Actives
        DbSet<ActivePowerModeConfig> ActivePowerModeConfigs { get; set; }
        DbSet<FrequencyDroopModeConfig> FrequencyDroopModeConfigs { get; set; }
        DbSet<FrequencyWattModeConfig> FrequencyWattModeConfigs { get; set; }
        DbSet<MaximumActivePowerLimitModeConfig> MaximumActivePowerLimitModeConfigs { get; set; }
        // Operating closed modes Voltage Actives
        DbSet<VoltageWattDroopModeConfig> VoltageWattDroopModeConfigs { get; set; }
        // Operating closed modes Voltage Reactives
        DbSet<PowerFactorModeConfig> PowerFactorModeConfigs { get; set; }
        DbSet<ReactivePowerModeConfig> ReactivePowerModeConfigs { get; set; }
        DbSet<VoltageModeConfig> VoltageModeConfigs { get; set; }
        DbSet<VoltageVarDroopModeConfig> VoltageVarDroopModeConfigs { get; set; }

        // Operating open modes
        DbSet<MaintenanceModeConfig> MaintenanceModeConfigs { get; set; }
        // Operating open modes Energy Storages
        DbSet<MaximumPowerPointTrackingModeConfig> MaximumPowerPointTrackingModeConfigs { get; set; }
        // Operating open modes Frequency Actives
        DbSet<ActivePowerFixedModeConfig> ActivePowerFixedModeConfigs { get; set; }
        // Operating open modes Voltage Reactives
        DbSet<ReactivePowerFixedModeConfig> ReactivePowerFixedModeConfigs { get; set; }


        /// <summary>
        /// Save changes.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task result contains the number of state entries written to the underlying database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
