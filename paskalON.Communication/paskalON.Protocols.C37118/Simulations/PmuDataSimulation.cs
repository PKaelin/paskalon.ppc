// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118.Simulations
{
    /// <summary>
    /// PMU data simulation by one PMU stream.
    /// </summary>
    public class PmuDataSimulation : IPmuDataSimulation
    {
        /// <inheritdoc/>        
        public ushort StreamId { get; init; }


        /// <inheritdoc/>
        public float Frequency { get; init; }


        /// <inheritdoc/>
        public float FrequencyRateOfChange { get; init; }


        /// <inheritdoc/>
        public IReadOnlyList<PhasorMeasurement> Phasors { get; init; } = Array.Empty<PhasorMeasurement>();


        /// <inheritdoc/>
        public IReadOnlyList<AnalogMeasurement> Analogs { get; init; } = Array.Empty<AnalogMeasurement>();
    }
}
