// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;

namespace paskalON.Devices.Dto.Meters.PowerMeters
{
    /// <summary>
    /// Data Transfer Object for a Circuit Power Meter.
    /// </summary>
    public record PmCircuitDto : DeviceBase<PmCircuitDefinitionDto, PmCircuitCoreDto, PmCircuitDetailDto>
    {
        /// <summary>
        /// Constructor of <see cref="PmCircuitDto"/>.
        /// </summary>
        /// <param name="definition">Power Meter definition DTO.</param>
        public PmCircuitDto(PmCircuitDefinitionDto definition) : base(definition)
        {
        }
    }
}
