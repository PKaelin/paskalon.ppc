// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.PowerConversionSystems
{
    public class PcsDto : DeviceBase<PcsDefinitionDto, PcsCoreDto, PcsDetailDto>
    {
        public PcsDto(PcsDefinitionDto definition) : base(definition)
        {
        }
    }
}
