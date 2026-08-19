// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyStorages.Batteries
{
    /// <summary>
    /// Data Transfer Object for a Battery Bank.
    /// </summary>
    public class BbDto : DeviceBase<BbDefinitionDto, BbCoreDto, BbDetailDto>
    {
        /// <summary>
        /// Constructor of <see cref="BbDto"/>.
        /// </summary>
        /// <param name="definition">Battery Bank definition DTO.</param>
        public BbDto(BbDefinitionDto definition) : base(definition)
        {
        }
    }
}
