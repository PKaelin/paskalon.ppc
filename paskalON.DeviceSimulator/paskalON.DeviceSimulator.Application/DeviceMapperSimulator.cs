// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyResources.Solars;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.DeviceSimulator.Dto.Ders;
using paskalON.DeviceSimulator.Dto.Endpoints;
using paskalON.DeviceSimulator.Dto.EnergyResources.Solars;
using paskalON.DeviceSimulator.Dto.EnergyStorages.Batteries;
using paskalON.DeviceSimulator.Dto.Meters.PowerMeters;
using paskalON.DeviceSimulator.Dto.PowerConversionSystems;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.C37118.Simulations;
using paskalON.Protocols.Modbus.Stores;
using System.Reflection;

namespace paskalON.DeviceSimulator.Application
{
    /// <summary>
    /// Maps domain device objects, stores and streams to their DTO representations.
    /// </summary>
    public class DeviceMapperSimulator
    {
        /// <summary>
        /// Simulation store registry.
        /// </summary>
        private readonly ISimulationStoreRegistry _stores;


        /// <summary>
        /// Simulation stream registry.
        /// </summary>
        private readonly ISimulationStreamRegistry _streams;


        /// <summary>
        /// Constructor of <see cref="DeviceMapperSimulator"/>.
        /// </summary>
        /// <param name="stores">Simulation store registry.</param>
        /// <param name="streams">Simulation stream registry.</param>
        public DeviceMapperSimulator(ISimulationStoreRegistry stores, ISimulationStreamRegistry streams)
        {
            ArgumentNullException.ThrowIfNull(stores);
            ArgumentNullException.ThrowIfNull(streams);

            _stores = stores;
            _streams = streams;
        }


        /// <summary>
        /// Maps a DER domain object to a DER DTO.
        /// </summary>
        public DerDto MapDer(Der der)
        {
            ArgumentNullException.ThrowIfNull(der);

            DerDto dto = new DerDto
            {
                Name = der.Name,
                SystemPowerMeters = der.SystemPowerMeters.Select(MapSystemPowerMeter).ToList(),
                AuxiliaryPowerMeters = der.AuxiliaryPowerMeters.Select(MapAuxiliaryPowerMeter).ToList(),
                ExternalPowerMeters = der.ExternalPowerMeters.Select(MapExternalPowerMeter).ToList()
            };

            dto.DerGroups = der.DerGroups.Select(MapDerGroup).ToList();

            return dto;
        }


        /// <summary>
        /// Maps a DER group domain object to a DER group DTO.
        /// </summary>
        public DerGroupDto MapDerGroup(DerGroup group)
        {
            ArgumentNullException.ThrowIfNull(group);

            return new DerGroupDto { Name = group.Name, DerCircuits = group.DerCircuits.Select(MapDerCircuit).ToList() };
        }


        /// <summary>
        /// Maps a DER circuit domain object to a DER circuit DTO.
        /// </summary>
        public DerCircuitDto MapDerCircuit(DerCircuit circuit)
        {
            ArgumentNullException.ThrowIfNull(circuit);

            return new DerCircuitDto
            {
                Name = circuit.Name,
                DerUnits = circuit.DerUnits.Select(MapDerUnit).ToList(),
                CircuitPowerMeter = circuit.CircuitPowerMeter is null ? null : MapCircuitPowerMeter(circuit.CircuitPowerMeter)
            };
        }


        /// <summary>
        /// Maps a DER unit domain object to its concrete DER unit DTO.
        /// </summary>
        public DerUnitDto MapDerUnit(DerUnit unit)
        {
            ArgumentNullException.ThrowIfNull(unit);

            return unit switch
            {
                DerBatteryStorageUnit batteryStorageUnit => MapDerBatteryStorageUnit(batteryStorageUnit),
                DerSolarUnit solarUnit => MapDerSolarUnit(solarUnit),
                _ => throw new InvalidOperationException($"Unsupported DER unit type '{unit.GetType().FullName}'.")
            };
        }


        /// <summary>
        /// Maps a battery storage unit domain object to a DTO.
        /// </summary>
        public DerBatteryStorageUnitDto MapDerBatteryStorageUnit(DerBatteryStorageUnit unit)
        {
            ArgumentNullException.ThrowIfNull(unit);

            DerBatteryStorageUnitDto dto = new DerBatteryStorageUnitDto
            {
                Name = unit.Name,
                PowerConversionSystem = MapPowerConversionSystem(unit.PowerConversionSystem ?? throw new InvalidOperationException("Battery storage unit has no power conversion system.")),
                IsInMaintenanceMode = unit.IsInMaintenanceMode,
                BatteryBanks = unit.BatteryBanks.Select(MapBatteryBank).ToList()
            };

            return dto;
        }


        /// <summary>
        /// Maps a solar unit domain object to a DTO.
        /// </summary>
        public DerSolarUnitDto MapDerSolarUnit(DerSolarUnit unit)
        {
            ArgumentNullException.ThrowIfNull(unit);

            return new DerSolarUnitDto
            {
                Name = unit.Name,
                PowerConversionSystem = MapPowerConversionSystem(unit.PowerConversionSystem ?? throw new InvalidOperationException("Solar unit has no power conversion system.")),
                IsInMaintenanceMode = unit.IsInMaintenanceMode,
                SolarPanels = unit.SolarPanels.Select(MapSolarPanel).ToList(),
                NumberOfPanels = unit.NumberOfPanels
            };
        }


        /// <summary>
        /// Maps a PCS domain object, including its endpoint.
        /// </summary>
        public PcsDto MapPowerConversionSystem(PowerConversionSystemBase device)
        {
            ArgumentNullException.ThrowIfNull(device);

            return new PcsDto { DeviceId = device.DeviceId, Name = device.Name, Endpoints = MapModbusEndpoints(device) };
        }


        /// <summary>
        /// Maps a battery bank domain object, including its endpoint.
        /// </summary>
        public BbDto MapBatteryBank(BatteryBankBase device)
        {
            ArgumentNullException.ThrowIfNull(device);

            return new BbDto { DeviceId = device.DeviceId, Name = device.Name, Endpoints = MapModbusEndpoints(device) };
        }


        /// <summary>
        /// Maps a solar panel domain object, including its endpoint.
        /// </summary>
        public PvDto MapSolarPanel(SolarPanelBase solarPanel)
        {
            ArgumentNullException.ThrowIfNull(solarPanel);

            return new PvDto { DeviceId = solarPanel.DeviceId, Name = solarPanel.Name, Endpoints = new List<ModbusEndpointDto>() };
        }


        /// <summary>
        /// Maps a system power meter domain object, including its endpoint.
        /// </summary>
        public PmSystemDto MapSystemPowerMeter(SystemPowerMeter device)
        {
            ArgumentNullException.ThrowIfNull(device);

            return new PmSystemDto { DeviceId = device.DeviceId, Name = device.Name, Endpoints = MapC37Endpoints(device) };
        }


        /// <summary>
        /// Maps a external power meter domain object, including its endpoint.
        /// </summary>
        public PmExternalDto MapExternalPowerMeter(ExternalPowerMeter device)
        {
            ArgumentNullException.ThrowIfNull(device);

            return new PmExternalDto { DeviceId = device.DeviceId, Name = device.Name, Endpoints = MapC37Endpoints(device) };
        }


        /// <summary>
        /// Maps a auxiliary power meter domain object, including its endpoint.
        /// </summary>
        public PmAuxiliaryDto MapAuxiliaryPowerMeter(AuxiliaryPowerMeter device)
        {
            ArgumentNullException.ThrowIfNull(device);

            return new PmAuxiliaryDto { DeviceId = device.DeviceId, Name = device.Name, Endpoints = MapC37Endpoints(device) };
        }


        /// <summary>
        /// Maps a circuit power meter domain object, including its endpoint.
        /// </summary>
        public PmCircuitDto MapCircuitPowerMeter(CircuitPowerMeter device)
        {
            ArgumentNullException.ThrowIfNull(device);

            return new PmCircuitDto { DeviceId = device.DeviceId, Name = device.Name, Endpoints = MapC37Endpoints(device) };
        }


        /// <summary>
        /// Create a list of Modbus endpoints according to a Modbus device.
        /// </summary>
        private List<ModbusEndpointDto> MapModbusEndpoints(object device)
        {
            Type registerEnum = FindRegisterEnum(device.GetType());
            IModbusDataStore store = FindStore(device);

            return Enum.GetValues(registerEnum).Cast<object>().Select(register =>
            {
                ushort address = Convert.ToUInt16(register);
                ushort[] values = store.HoldingRegisters.ReadPoints(address, 1);

                return new ModbusEndpointDto
                {
                    Name = register.ToString()!,
                    Address = address,
                    DataType = ModbusDataType.MbUint16,
                    Value = values[0]
                };
            }).ToList();
        }


        /// <summary>
        /// Create a list of C37 endpoints according to a C37 device.
        /// </summary>
        private List<C37EndpointDto> MapC37Endpoints(PowerMeterBase device)
        {
            PmuDataSimulation stream = _streams.Streams
                .Where(entry => entry.Key.Address == device.TargetAddress && entry.Key.Port == device.TargetPort
                    && entry.Key.StationName == device.TargetStationName && entry.Key.StreamId == device.TargetStreamId)
                .Select(entry => entry.Value).FirstOrDefault()
                ?? throw new InvalidOperationException($"No C37 simulation stream exists for device '{device.Name}'.");

            if (device.C37Map is null)
            {
                return new List<C37EndpointDto>();
            }

            return device.C37Map.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Select(property => new { Property = property, Endpoint = property.GetValue(device.C37Map) as string })
                .Where(item => string.IsNullOrWhiteSpace(item.Endpoint) == false)
                .OrderBy(item => item.Property.Name)
                .Select(item => MapC37Endpoint(item.Property.Name, item.Endpoint!, stream)).ToList();
        }


        /// <summary>
        /// Map the C37 endpoint
        /// </summary>
        /// <param name="name">Name of the endpoint.</param>
        /// <param name="stream">Stream of the endpoint.</param>
        /// <returns>C37 Endpoint</returns>
        private C37EndpointDto MapC37Endpoint(string name, string endpoint, PmuDataSimulation stream)
        {
            if (stream.Analogs.TryGetValue(endpoint, out AnalogMeasurement? analog))
            {
                return new C37EndpointDto { Name = name, Endpoint = endpoint, SignalType = C37SignalType.Analog, Value = analog.Measurement };
            }

            if (stream.Phasors.TryGetValue(endpoint, out PhasorMeasurement? phasor))
            {
                return new C37EndpointDto { Name = name, Endpoint = endpoint, SignalType = C37SignalType.Phasor, Value = phasor.Magnitude };
            }

            return new C37EndpointDto { Name = name, Endpoint = endpoint, SignalType = C37SignalType.Analog, Value = null };
        }


        /// <summary>
        /// Finds the store of a device.
        /// </summary>
        /// <param name="device">The device to find the store.</param>
        /// <returns>Modbus data store interface.</returns>
        private IModbusDataStore FindStore(object device)
        {
            string address = (string)device.GetType().GetProperty("TargetAddress")!.GetValue(device)!;
            int port = (int)device.GetType().GetProperty("TargetPort")!.GetValue(device)!;

            return _stores.Stores.Where(entry => entry.Key.Address == address && entry.Key.Port == port)
                .Select(entry => entry.Value).FirstOrDefault() ??
                throw new InvalidOperationException($"No Modbus simulation store exists for device '{device}'.");
        }


        /// <summary>
        /// Finds the devices description class and its register enum.
        /// </summary>
        /// <param name="deviceType">The device type.</param>
        /// <returns>Returns the register enum of the devices description.</returns>
        private static Type FindRegisterEnum(Type deviceType)
        {
            string descriptionName = deviceType.Name.Replace("Proxy", "Description", StringComparison.Ordinal);
            Type? descriptionType = deviceType.Assembly.GetTypes().FirstOrDefault(type => type.Name == descriptionName);
            Type? registerEnum = descriptionType?.GetNestedType("Register", BindingFlags.Public);

            return registerEnum is not null && registerEnum.IsEnum ? registerEnum :
                throw new InvalidOperationException($"No Register enum was found for device type '{deviceType.FullName}'.");
        }
    }
}
