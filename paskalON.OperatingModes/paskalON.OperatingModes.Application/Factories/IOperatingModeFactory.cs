// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Dto.Ders;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Application.Factories
{
    /// <summary>
    /// Factory interface for creating operating mode instances.
    /// </summary>
    public interface IOperatingModeFactory
    {
        /// <summary>
        /// Creates an instance of OperatingModeBase based on the provided configuration and DER information.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="timeProvider">The time provider instance.</param>
        /// <param name="publisher">The metrics publisher instance.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode base configuration.</param>
        /// <param name="der">The DER information.</param>
        /// <returns>The created OperatingModeBase instance.</returns>
        OperatingModeBase Create(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig,
            OperatingModeBaseConfig config, DerDto der);
    }
}
