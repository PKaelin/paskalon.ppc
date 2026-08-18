// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyResources.Solars
{
    public class PvDto : DeviceBase<PvDefinitionDto, PvCoreDto, PvDetailDto>
    {
        public PvDto(PvDefinitionDto definition) : base(definition)
        {
        }
    }
}
