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
        /// Selected operating modes ordered by ascending priority.
        /// </summary>
        IReadOnlyCollection<OperatingModeBase> AvailableOperatingModes { get; }


        /// <summary>
        /// Selected operating modes ordered by ascending priority.
        /// </summary>
        IReadOnlyCollection<OperatingModeBase> SelectedOperatingModes { get; }


        /// <summary>
        /// Adds an initialized operating mode to the selected stack.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode to add.</param>
        /// <param name="priority">The unique priority to assign.</param>
        void AddOperatingMode(string operatingModeName, int priority);


        /// <summary>
        /// Moves a selected operating mode to a new priority.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode to move.</param>
        /// <param name="priority">The unique priority to assign.</param>
        void MoveOperatingMode(string operatingModeName, int priority);


        /// <summary>
        /// Removes an operating mode from the selected stack.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode to remove.</param>
        void RemoveOperatingMode(string operatingModeName);


        /// <summary>
        /// Initializes the operating mode manager using the provided repository.
        /// </summary>
        /// <param name="repository">The operating mode repository to initialize the manager with.</param>
        /// <param name="systemConfig">The system configuration shared by all operating modes.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Initialize(IOperatingModeRepository repository, SystemConfig systemConfig);
    }
}
