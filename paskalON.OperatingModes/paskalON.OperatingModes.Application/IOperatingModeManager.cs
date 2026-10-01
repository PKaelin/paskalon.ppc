// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Infrastructure.Storage.Repositories;
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
        /// Collection of all configured operating modes.
        /// </summary>
        IReadOnlyCollection<OperatingModeBase> OperatingModes { get; }


        /// <summary>
        /// Initializes the operating mode manager using the provided repository.
        /// </summary>
        /// <param name="repository">The operating mode repository to initialize the manager with.</param>
        /// <param name="systemConfig">The system configuration shared by all operating modes.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Initialize(IOperatingModeRepository repository, SystemConfig systemConfig);
    }
}
