// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Application.Overrides;
using paskalON.OperatingModes.Application.Stacks;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.OperatingModes.Infrastructure.Storage.Repositories;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Application
{
    /// <summary>
    /// Interface for managing operating mode operations, configurations, and telemetry.
    /// </summary>
    public interface IOperatingModeManager
    {
        /// <summary>
        /// Collection of metrics publishers for telemetry.
        /// </summary>
        ICollection<IMetricsPublisher> MetricsPublishers { get; }


        /// <summary>
        /// Gets the stack manager responsible for managing operating mode stack.
        /// </summary>
        IStackManager StackManager { get; }


        /// <summary>
        /// Gets the override manager responsible for managing the override slot.
        /// </summary>
        IOverrideManager OverrideManager { get; }


        /// <summary>
        /// Gets the system operating mode control or null if the manager is not initialized.
        /// </summary>
        ISystemOperatingModeControl? SystemOperatingModeControl { get; }


        /// <summary>
        /// Initializes the operating mode manager using the provided repository.
        /// </summary>
        /// <param name="repository">The operating mode repository to initialize the manager with.</param>
        /// <param name="systemConfig">The system configuration shared by all operating modes.</param>
        /// <param name="setpointsPublisher">Publisher of the applied setpoints of the system operating mode control.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Initialize(IOperatingModeRepository repository, SystemConfig systemConfig, ISetpointsPublisher setpointsPublisher);


        /// <summary>
        /// Updates the active and reactive power for the operating modes.
        /// </summary>
        /// <param name="activePower">The active power to set.</param>
        /// <param name="reactivePower">The reactive power to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetSystemPowerSetpoints(ActivePower activePower, ReactivePower reactivePower);
    }
}
