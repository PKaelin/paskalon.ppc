// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Client.Registers;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.PowerConversionSystems;

namespace paskalON.Devices.Client
{
    public interface IDeviceClient
    {
        DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> PcsRegisters { get; }
        DeviceRegister<BbDto, BbDefinitionDto, BbCoreDto, BbDetailDto> BbRegisters { get; }
        ICollection<PcsDto> Pcs { get; }
        ICollection<BbDto> Bbs { get; }
    }
}
