// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyResources.Solars;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Domain.PowerConversionSystems;

namespace paskalON.Devices.Application
{
    public interface IDeviceManager
    {
        Der Der { get; }

        ICollection<PowerConversionSystemBase> PowerConversionSystems { get; }

        ICollection<BatteryBankBase> BatteryBanks { get; }

        ICollection<SolarPanelBase> Solars { get; }

        ICollection<ExternalPowerMeter> ExternalPowerMeters { get; }

        ICollection<AuxiliaryPowerMeter> AuxiliaryPowerMeters { get; }

        ICollection<SystemPowerMeter> SystemPowerMeters { get; }

        ICollection<CircuitPowerMeter> CircuitPowerMeters { get; }

        Task LoadDer();
    }
}
