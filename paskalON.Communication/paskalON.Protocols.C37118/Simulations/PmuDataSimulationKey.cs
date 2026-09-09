// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118.Simulations
{
    /// <summary>
    /// PMU data key to identify PMU data.
    /// </summary>
    public record PmuDataSimulationKey
    {
        /// <summary>
        /// Port of the PMU.
        /// </summary>
        public int Port { get; init; }


        /// <summary>
        /// PMU stream identifier.
        /// </summary>
        public ushort StreamId { get; init; }
    }
}
