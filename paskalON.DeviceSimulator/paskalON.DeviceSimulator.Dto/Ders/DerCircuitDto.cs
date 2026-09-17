// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Dto.Meters.PowerMeters;

namespace paskalON.DeviceSimulator.Dto.Ders
{
    /// <summary>
    /// Data Transfer Object for DER circuit.
    /// </summary>
    public record DerCircuitDto
    {
        /// <summary>
        /// Name of the Distributed Energy Resource (DER) Data Transfer Object (DTO.
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// List of DER units.
        /// </summary>
        public List<DerUnitDto> DerUnits { get; set; } = new List<DerUnitDto>();


        // TODO: Add CircuitBreaker


        /// <summary>
        /// Optional circuit power meter.
        /// </summary>
        /// <remarks>
        /// Circuit power meter is a power meter just for this specific circuit.
        /// It is sometimes called feeder meter.
        /// </remarks>
        public PmCircuitDto? CircuitPowerMeter { get; init; }
    }
}
