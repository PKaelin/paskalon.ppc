// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Collections.ObjectModel;

namespace paskalON.Protocols.C37118.Simulations
{
    /// <summary>
    /// PMU data simulation by one PMU stream.
    /// </summary>
    public class PmuDataSimulation : IPmuDataSimulation
    {
        /// <inheritdoc/>
        public required string StationName { get; init; }


        /// <inheritdoc/>        
        public required ushort StreamId { get; init; }


        /// <inheritdoc/>
        public float Frequency { get; set; }


        /// <inheritdoc/>
        public float FrequencyRateOfChange { get; set; }


        /// <inheritdoc/>
        public ReadOnlyDictionary<string, PhasorMeasurement> Phasors { get; init; } = ReadOnlyDictionary<string, PhasorMeasurement>.Empty;


        /// <inheritdoc/>
        public ReadOnlyDictionary<string, AnalogMeasurement> Analogs { get; init; } = ReadOnlyDictionary<string, AnalogMeasurement>.Empty;
    }
}
