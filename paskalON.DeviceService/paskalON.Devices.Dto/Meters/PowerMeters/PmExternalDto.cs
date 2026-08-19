// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;

namespace paskalON.Devices.Dto.Meters.PowerMeters
{
    /// <summary>
    /// Data Transfer Object for an External Power Meter.
    /// </summary>
    public record PmExternalDto : DeviceBase<PmExternalDefinitionDto, PmExternalCoreDto, PmExternalDetailDto>
    {
        /// <summary>
        /// Constructor of <see cref="PmExternalDto"/>.
        /// </summary>
        /// <param name="definition">Power Meter definition DTO.</param>
        public PmExternalDto(PmExternalDefinitionDto definition) : base(definition)
        {
        }
    }
}
