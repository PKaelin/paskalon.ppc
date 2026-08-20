// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.EnergyResources.Solars;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.PowerConversionSystems;

namespace paskalON.Devices.Client
{
    public interface IDeviceClient
    {
        DerDto Der { get; }
        ICollection<PcsDto> PowerConversionSystems { get; }
        ICollection<BbDto> BatteryBanks { get; }
        ICollection<PvDto> SolarPanels { get; }
        ICollection<PmExternalDto> ExternalPowerMeters { get; }
        ICollection<PmAuxiliaryDto> AuxiliaryPowerMeters { get; }
        ICollection<PmCircuitDto> CircuitPowerMeters { get; }

    }
}
