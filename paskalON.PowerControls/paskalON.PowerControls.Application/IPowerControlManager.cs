// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Systems;
using paskalON.PowerControls.Domain.Systems;
using paskalON.PowerControls.Infrastructure.Storage;
using paskalON.Telemetry;

namespace paskalON.PowerControls.Application
{
    /// <summary>
    /// Interface for managing power control operations, configurations, and telemetry.
    /// </summary>
    public interface IPowerControlManager
    {
        /// <summary>
        /// Collection of metrics publishers for telemetry.
        /// </summary>
        ICollection<IMetricsPublisher> MetricsPublishers { get; }


        /// <summary>
        /// List of system power control configurations.
        /// </summary>
        IReadOnlyList<SystemPowerControlConfig> SystemPowerControlConfigurations { get; }


        /// <summary>
        /// List of DER unit power control configurations.
        /// </summary>
        IReadOnlyList<DerUnitPowerControlConfig> DerUnitPowerControlConfigurations { get; }


        /// <summary>
        /// List of DER unit energy storage power control configurations.
        /// </summary>
        IReadOnlyList<DerUnitEnergyStoragePowerControlConfig> DerUnitEnergyStoragePowerControlConfigurations { get; }


        /// <summary>
        /// List of constraint configurations.
        /// </summary>
        IReadOnlyList<ConstraintBaseConfig> ConstraintConfigurations { get; }


        /// <summary>
        /// System power control instance.
        /// </summary>
        SystemPowerControl? SystemPowerControl { get; }


        /// <summary>
        /// Initializes the power control manager with the provided context.
        /// </summary>
        /// <param name="context">The power control context.</param>
        /// <returns>Task.</returns>
        Task Initialize(PowerControlContext context);


        /// <summary>
        /// Sets the system power target for the power control manager.
        /// </summary>
        /// <param name="activePower">The active power target.</param>
        /// <param name="reactivePower">The reactive power target.</param>
        /// <returns>Task.</returns>
        Task SetSystemPowerTarget(ActivePower activePower, ReactivePower reactivePower);
    }
}
