// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Collections.ObjectModel;

namespace paskalON.Protocols.C37118.Simulations
{
    /// <summary>
    /// Describes the values produced by one simulated PMU stream.
    /// </summary>
    public interface IPmuDataSimulation
    {
        /// <summary>
        /// Station name identifier of PMU or PDC.
        /// </summary>
        string StationName { get; init; }


        /// <summary>
        /// PMU stream identifier.
        /// </summary>
        ushort StreamId { get; }


        /// <summary>
        /// Nominal frequency in hertz.
        /// </summary>
        float Frequency { get; }


        /// <summary>
        /// Frequency rate of change.
        /// </summary>
        float FrequencyRateOfChange { get; }


        /// <summary>
        /// Configured phasor measurements.
        /// </summary>
        ReadOnlyDictionary<string, PhasorMeasurement> Phasors { get; }


        /// <summary>
        /// Configured analog measurements.
        /// </summary>
        ReadOnlyDictionary<string, AnalogMeasurement> Analogs { get; }
    }
}
