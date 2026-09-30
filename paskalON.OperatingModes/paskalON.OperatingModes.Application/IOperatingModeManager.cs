// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
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
    }
}
