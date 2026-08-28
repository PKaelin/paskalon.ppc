// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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
