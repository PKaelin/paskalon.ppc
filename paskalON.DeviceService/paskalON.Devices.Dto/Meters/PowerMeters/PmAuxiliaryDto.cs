// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;

namespace paskalON.Devices.Dto.Meters.PowerMeters
{
    /// <summary>
    /// Data Transfer Object for a Auxiliary Power Meter.
    /// </summary>
    public record PmAuxiliaryDto : DeviceBase<PmAuxiliaryDefinitionDto, PmAuxiliaryCoreDto, PmAuxiliaryDetailDto>
    {
        /// <summary>
        /// Constructor of <see cref="PmAuxiliaryDto"/>.
        /// </summary>
        /// <param name="definition">Power Meter definition DTO.</param>
        public PmAuxiliaryDto(PmAuxiliaryDefinitionDto definition) : base(definition)
        {
        }
    }
}
