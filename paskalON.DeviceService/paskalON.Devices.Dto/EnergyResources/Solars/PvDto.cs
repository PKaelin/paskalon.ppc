// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyResources.Solars
{
    /// <summary>
    /// Data Transfer Object for a Photovoltaic (Solar) energy resource.
    /// </summary>
    public record PvDto : DeviceBase<PvDefinitionDto, PvCoreDto, PvDetailDto>
    {
        /// <summary>
        /// Constructor of <see cref="PvDto"/>.
        /// </summary>
        /// <param name="definition">PV (Solar) definition DTO.</param>
        public PvDto(PvDefinitionDto definition) : base(definition)
        {
        }
    }
}
