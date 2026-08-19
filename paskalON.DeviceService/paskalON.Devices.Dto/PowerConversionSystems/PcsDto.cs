// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.PowerConversionSystems
{
    /// <summary>
    /// Data Transfer Object for a Power Conversion System.
    /// </summary>
    public class PcsDto : DeviceBase<PcsDefinitionDto, PcsCoreDto, PcsDetailDto>
    {
        /// <summary>
        /// Constructor of <see cref="PcsDto"/>.
        /// </summary>
        /// <param name="definition">Power Conversion System definition DTO.</param>
        public PcsDto(PcsDefinitionDto definition) : base(definition)
        {
        }
    }
}
