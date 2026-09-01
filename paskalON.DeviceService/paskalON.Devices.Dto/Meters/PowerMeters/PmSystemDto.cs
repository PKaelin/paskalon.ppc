// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;

namespace paskalON.Devices.Dto.Meters.PowerMeters
{
    /// <summary>
    /// Data Transfer Object for a System Power Meter.
    /// </summary>
    public record PmSystemDto : DeviceBase<PmSystemDefinitionDto, PmSystemCoreDto, PmSystemDetailDto>
    {
        /// <summary>
        /// Constructor of <see cref="PmSystemDto"/>.
        /// </summary>
        /// <param name="definition">Power Meter definition DTO.</param>
        public PmSystemDto(PmSystemDefinitionDto definition) : base(definition)
        {
        }
    }
}
