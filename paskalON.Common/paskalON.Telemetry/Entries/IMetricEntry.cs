// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using System.Diagnostics.Metrics;

namespace paskalON.Telemetry.Entries
{
    /// <summary>
    /// Interface for metric entries.
    /// </summary>
    public interface IMetricEntry
    {
        /// <summary>
        /// Instance to use for the update.
        /// </summary>
        object Instance { get; }


        /// <summary>
        /// Name of the metric.
        /// </summary>
        string Name { get; init; }


        /// <summary>
        /// Interval of the metric.
        /// </summary>
        int Interval { get; init; }


        /// <summary>
        /// Metric type.
        /// </summary>
        MetricType MetricType { get; init; }


        /// <summary>
        /// Metric instrument.
        /// </summary>
        Instrument Instrument { get; init; }


        /// <summary>
        /// Updates the metric value.
        /// </summary>
        void Update();
    }
}
