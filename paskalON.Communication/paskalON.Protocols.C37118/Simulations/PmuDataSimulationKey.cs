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
        /// Address of the simulated C37 endpoint.
        /// </summary>
        public string Address { get; init; } = string.Empty;

        /// <summary>
        /// Port of the PMU.
        /// </summary>
        public required int Port { get; init; }


        /// <summary>
        /// Station name of PMU or PDC.
        /// </summary>
        public required string StationName { get; init; }


        /// <summary>
        /// PMU stream identifier.
        /// </summary>
        public required ushort StreamId { get; init; }
    }
}
