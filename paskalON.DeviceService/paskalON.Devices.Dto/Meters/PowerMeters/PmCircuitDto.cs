// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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
