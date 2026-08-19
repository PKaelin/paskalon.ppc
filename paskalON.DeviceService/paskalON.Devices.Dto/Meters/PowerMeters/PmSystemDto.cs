// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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
