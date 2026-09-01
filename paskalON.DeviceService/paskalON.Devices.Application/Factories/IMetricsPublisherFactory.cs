// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Telemetry;

namespace paskalON.Devices.Application.Factories
{
    /// <summary>
    /// Metrics publisher factory interface definition.
    /// </summary>
    public interface IMetricsPublisherFactory
    {
        /// <summary>
        /// Create an IMetricsPublisher.
        /// </summary>
        /// <returns>The IMetricsPublisher implementation.</returns>
        IMetricsPublisher Create();
    }
}
