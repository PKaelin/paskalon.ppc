// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyStorages.Batteries
{
    public class BbDto : DeviceBase<BbDefinitionDto, BbCoreDto, BbDetailDto>
    {
        public BbDto(BbDefinitionDto definition) : base(definition)
        {
        }
    }
}
