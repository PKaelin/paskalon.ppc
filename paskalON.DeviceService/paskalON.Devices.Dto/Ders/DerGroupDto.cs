// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.Ders
{
    /// <summary>
    /// Data Transfer Object for DER group.
    /// </summary>
    public record DerGroupDto
    {
        /// <summary>
        /// List of DERs that are grouped in a circuit.
        /// A circuit can have a breaker and a meter.
        /// </summary>
        public List<DerCircuitDto> DerCircuits { get; set; } = new List<DerCircuitDto>();
    }
}
