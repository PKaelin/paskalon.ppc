// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Application;
using paskalON.Devices.Client.Registers;
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.EnergyResources.Solars;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;
using paskalON.Devices.Dto.PowerConversionSystems;

namespace paskalON.Devices.Client
{
    public class DeviceClient : IDeviceClient
    {
        private readonly ILogger _logger;
        private readonly IDeviceServer _deviceServer;

        public DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> PcsRegisters { get; }
        public DeviceRegister<BbDto, BbDefinitionDto, BbCoreDto, BbDetailDto> BbRegisters { get; }
        public DeviceRegister<PvDto, PvDefinitionDto, PvCoreDto, PvDetailDto> PvRegisters { get; }
        public DeviceRegister<PmExternalDto, PmExternalDefinitionDto, PmExternalCoreDto, PmExternalDetailDto> PmExternalRegisters { get; }
        public DeviceRegister<PmAuxiliaryDto, PmAuxiliaryDefinitionDto, PmAuxiliaryCoreDto, PmAuxiliaryDetailDto> PmAuxiliaryRegisters { get; }
        public DeviceRegister<PmCircuitDto, PmCircuitDefinitionDto, PmCircuitCoreDto, PmCircuitDetailDto> PmCircuitRegisters { get; }
        public DeviceRegister<PmSystemDto, PmSystemDefinitionDto, PmSystemCoreDto, PmSystemDetailDto> PmSystemRegisters { get; }

        public DerDto Der { get; private set; } = new DerDto();
        public ICollection<PcsDto> Pcs { get => PcsRegisters.Devices; }
        public ICollection<BbDto> Bbs { get => BbRegisters.Devices; }
        public ICollection<PvDto> Pvs { get => PvRegisters.Devices; }
        public ICollection<PmExternalDto> PmExternals { get => PmExternalRegisters.Devices; }
        public ICollection<PmAuxiliaryDto> PmAuxiliaries { get => PmAuxiliaryRegisters.Devices; }
        public ICollection<PmCircuitDto> PmCircuits { get => PmCircuitRegisters.Devices; }


        public DeviceClient(ILogger logger, IDeviceServer deviceServer)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceServer);

            _logger = logger;
            _deviceServer = deviceServer;
            // PCS, BB, PV registers
            PcsRegisters = new DeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(_logger);
            BbRegisters = new DeviceRegister<BbDto, BbDefinitionDto, BbCoreDto, BbDetailDto>(_logger);
            PvRegisters = new DeviceRegister<PvDto, PvDefinitionDto, PvCoreDto, PvDetailDto>(_logger);
            // Meter registers
            PmExternalRegisters = new DeviceRegister<PmExternalDto, PmExternalDefinitionDto, PmExternalCoreDto, PmExternalDetailDto>(_logger);
            PmAuxiliaryRegisters = new DeviceRegister<PmAuxiliaryDto, PmAuxiliaryDefinitionDto, PmAuxiliaryCoreDto, PmAuxiliaryDetailDto>(_logger);
            PmCircuitRegisters = new DeviceRegister<PmCircuitDto, PmCircuitDefinitionDto, PmCircuitCoreDto, PmCircuitDetailDto>(_logger);
            PmSystemRegisters = new DeviceRegister<PmSystemDto, PmSystemDefinitionDto, PmSystemCoreDto, PmSystemDetailDto>(_logger);
        }


        public async Task Initialize()
        {
            Der = await _deviceServer.GetDer();

            // Power meters
            Der.ExternalPowerMeters.ForEach((d) => { PmExternalRegisters.Add(new PmExternalDto(d.Definition)); });
            Der.AuxiliaryPowerMeters.ForEach((d) => { PmAuxiliaryRegisters.Add(new PmAuxiliaryDto(d.Definition)); });
            Der.DerGroups.SelectMany(g => g.DerCircuits).ToList().ForEach((d) =>
            {
                if (d.CircuitPowerMeter != null) PmCircuitRegisters.Add(new PmCircuitDto(d.CircuitPowerMeter.Definition));
            });
            Der.SystemPowerMeters.ForEach((d) => { PmSystemRegisters.Add(new PmSystemDto(d.Definition)); });

            // PCS
            Der.DerGroups.SelectMany(g => g.DerCircuits).SelectMany(c => c.DerUnits).OfType<DerBatteryStorageUnitDto>().ToList().ForEach(d =>
            {
                PcsRegisters.Add(new PcsDto(d.PowerConversionSystem.Definition));
            });
            Der.DerGroups.SelectMany(g => g.DerCircuits).SelectMany(c => c.DerUnits).OfType<DerSolarUnitDto>().ToList().ForEach(d =>
            {
                PcsRegisters.Add(new PcsDto(d.PowerConversionSystem.Definition));
            });

            // Battery Banks
            Der.DerGroups.SelectMany(g => g.DerCircuits).SelectMany(c => c.DerUnits).OfType<DerBatteryStorageUnitDto>()
                .SelectMany(b => b.BatteryBanks).ToList().ForEach(d =>
                {
                    BbRegisters.Add(new BbDto(d.Definition));
                });

            // Solars
            Der.DerGroups.SelectMany(g => g.DerCircuits).SelectMany(c => c.DerUnits).OfType<DerSolarUnitDto>()
                .SelectMany(s => s.SolarPanels).ToList().ForEach(d =>
                {
                    PvRegisters.Add(new PvDto(d.Definition));
                });
        }
    }
}
