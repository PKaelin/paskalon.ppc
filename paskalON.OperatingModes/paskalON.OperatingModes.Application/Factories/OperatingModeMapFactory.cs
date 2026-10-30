// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.OpenModes;
using paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.SafetyFunctions;
using paskalON.OperatingModes.Domain.OpenModes;
using paskalON.OperatingModes.Domain.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Application.Factories
{
    /// <summary>
    /// Factory that maps the device data to the input functions of the operating mode maps.
    /// </summary>
    /// <remarks>
    /// Plant-level available power excludes overridden DER units because they are removed from normal dispatch.
    /// The override mode itself only uses the available power of its DER unit.
    /// </remarks>
    public class OperatingModeMapFactory : IOperatingModeMapFactory
    {
        /// <inheritdoc/>
        public OperatingModeBaseMap? Create(OperatingModeBaseConfig config, DerDto der, IUnitOverrideLookup overrides)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(der);
            ArgumentNullException.ThrowIfNull(overrides);

            switch (config)
            {
                case MaintenanceModeConfig maintenanceConfig:
                    return CreateMaintenanceModeMap(maintenanceConfig, der);

                case ActivePowerFixedModeConfig:
                    return new ActivePowerFixedModeMap
                    {
                        AvailableActivePower = () => GetAvailableActivePower(GetDispatchableUnits(der, overrides)),
                        AvailableReactivePower = () => GetAvailableReactivePower(GetDispatchableUnits(der, overrides))
                    };

                case ReactivePowerFixedModeConfig:
                    return new ReactivePowerFixedModeMap
                    {
                        AvailableActivePower = () => GetAvailableActivePower(GetDispatchableUnits(der, overrides)),
                        AvailableReactivePower = () => GetAvailableReactivePower(GetDispatchableUnits(der, overrides))
                    };

                case MaximumPowerPointTrackingModeConfig:
                    return new MaximumPowerPointTrackingModeMap
                    {
                        AvailableActivePower = () => GetAvailableActivePower(GetDispatchableUnits(der, overrides)),
                        AvailableReactivePower = () => GetAvailableReactivePower(GetDispatchableUnits(der, overrides))
                    };

                case ActivePowerModeConfig:
                    return new ActivePowerModeMap
                    {
                        AvailableActivePower = () => GetAvailableActivePower(GetDispatchableUnits(der, overrides)),
                        AvailableReactivePower = () => GetAvailableReactivePower(GetDispatchableUnits(der, overrides)),
                        ActivePowerAtPoi = () => GetSystemActivePower(der)
                    };

                case ShutdownModeConfig:
                    return new OperatingModeBaseMap
                    {
                        AvailableActivePower = () => GetAvailableActivePower(GetDispatchableUnits(der, overrides)),
                        AvailableReactivePower = () => GetAvailableReactivePower(GetDispatchableUnits(der, overrides))
                    };

                case ActivePowerLimitModeConfig:
                    return new ActivePowerLimitModeMap
                    {
                        AvailableActivePower = () => GetAvailableActivePower(GetDispatchableUnits(der, overrides)),
                        AvailableReactivePower = () => GetAvailableReactivePower(GetDispatchableUnits(der, overrides)),
                        ActivePowerAtPoi = () => GetSystemActivePower(der)
                    };

                case ReactivePowerModeConfig:
                    return new ReactivePowerModeMap
                    {
                        AvailableActivePower = () => GetAvailableActivePower(GetDispatchableUnits(der, overrides)),
                        AvailableReactivePower = () => GetAvailableReactivePower(GetDispatchableUnits(der, overrides)),
                        ReactivePowerAtPoi = () => GetSystemReactivePower(der)
                    };

                case ReactivePowerLimitModeConfig:
                    return new ReactivePowerLimitModeMap
                    {
                        AvailableActivePower = () => GetAvailableActivePower(GetDispatchableUnits(der, overrides)),
                        AvailableReactivePower = () => GetAvailableReactivePower(GetDispatchableUnits(der, overrides)),
                        ReactivePowerAtPoi = () => GetSystemReactivePower(der)
                    };

                default:
                    return null;
            }
        }


        /// <inheritdoc/>
        public SystemOperatingModeControlMap CreateSystemMap(DerDto der, Func<string?> holdReason)
        {
            ArgumentNullException.ThrowIfNull(der);
            ArgumentNullException.ThrowIfNull(holdReason);

            return new SystemOperatingModeControlMap
            {
                ActivePowerAtPoi = () => GetSystemActivePower(der),
                ReactivePowerAtPoi = () => GetSystemReactivePower(der),
                HoldReason = holdReason
            };
        }


        /// <summary>
        /// Creates the maintenance mode map that only uses the available power of the overridden DER unit.
        /// </summary>
        /// <param name="config">The maintenance mode configuration.</param>
        /// <param name="der">The DER device data.</param>
        /// <returns>The maintenance mode map.</returns>
        /// <exception cref="InvalidOperationException">The DER unit does not exist.</exception>
        private static MaintenanceModeMap CreateMaintenanceModeMap(MaintenanceModeConfig config, DerDto der)
        {
            DerUnitDto unit = GetUnits(der).FirstOrDefault(derUnit => derUnit.Name == config.DerUnitName)
                ?? throw new InvalidOperationException($"DER unit '{config.DerUnitName}' not found in DER '{der.Name}'.");
            DerUnitDto[] units = new DerUnitDto[] { unit };

            return new MaintenanceModeMap
            {
                AvailableActivePower = () => GetAvailableActivePower(units),
                AvailableReactivePower = () => GetAvailableReactivePower(units),
                DerUnitName = unit.Name
            };
        }


        /// <summary>
        /// Gets all DER units of the DER.
        /// </summary>
        /// <param name="der">The DER device data.</param>
        /// <returns>All DER units.</returns>
        private static IEnumerable<DerUnitDto> GetUnits(DerDto der)
        {
            return der.DerGroups.SelectMany(group => group.DerCircuits).SelectMany(circuit => circuit.DerUnits);
        }


        /// <summary>
        /// Gets the DER units that are not overridden and therefore available for normal dispatch.
        /// </summary>
        /// <param name="der">The DER device data.</param>
        /// <param name="overrides">Lookup of overridden DER units.</param>
        /// <returns>The dispatchable DER units.</returns>
        private static IEnumerable<DerUnitDto> GetDispatchableUnits(DerDto der, IUnitOverrideLookup overrides)
        {
            return GetUnits(der).Where(unit => overrides.IsOverridden(unit.Name) is false);
        }


        /// <summary>
        /// Gets the power conversion system of a DER unit.
        /// </summary>
        /// <param name="unit">The DER unit.</param>
        /// <returns>The power conversion system or null if the DER unit has none.</returns>
        private static PcsDto? GetPowerConversionSystem(DerUnitDto unit)
        {
            return unit switch
            {
                DerBatteryStorageUnitDto battery => battery.PowerConversionSystem,
                DerSolarUnitDto solar => solar.PowerConversionSystem,
                _ => null
            };
        }


        /// <summary>
        /// Gets the power conversion system cores that are reachable.
        /// </summary>
        /// <param name="units">The DER units.</param>
        /// <returns>The power conversion system cores without communication error.</returns>
        private static IEnumerable<PcsCoreDto> GetCores(IEnumerable<DerUnitDto> units)
        {
            // Do not consider DER units that have no power conversion system or have a communication error.
            return units
                .Select(unit => GetPowerConversionSystem(unit)?.Core)
                .OfType<PcsCoreDto>()
                .Where(core => core.CommunicationError is false);
        }


        /// <summary>
        /// Gets the total available active power of the DER units.
        /// </summary>
        /// <param name="units">The DER units.</param>
        /// <returns>The total available active power or null if no DER unit reports it.</returns>
        private static ActivePower? GetAvailableActivePower(IEnumerable<DerUnitDto> units)
        {
            IEnumerable<PcsCoreDto> cores = GetCores(units);
            List<double> values = cores
                .Where(core => core.ActiveAvailablePower.HasValue)
                .Select(core => core.ActiveAvailablePower!.Value.Watts)
                .ToList();

            if (values.Count == 0)
            {
                return null;
            }

            return new ActivePower(values.Sum());
        }


        /// <summary>
        /// Gets the total available reactive power of the DER units.
        /// </summary>
        /// <remarks>
        /// Solar units are included for uniformity, they don't report reactive power in practice.
        /// </remarks>
        /// <param name="units">The DER units.</param>
        /// <returns>The total available reactive power or null if no DER unit reports it.</returns>
        private static ReactivePower? GetAvailableReactivePower(IEnumerable<DerUnitDto> units)
        {
            List<double> values = GetCores(units)
                .Where(core => core.ReactiveAvailablePower.HasValue)
                .Select(core => core.ReactiveAvailablePower!.Value.VoltAmperesReactive)
                .ToList();

            if (values.Count == 0)
            {
                return null;
            }

            return new ReactivePower(values.Sum());
        }


        /// <summary>
        /// Gets the measured active power at the point of interconnection from the system power meter.
        /// </summary>
        /// <param name="der">The DER device data.</param>
        /// <returns>The measured active power or null if no system power meter reports it.</returns>
        private static ActivePower? GetSystemActivePower(DerDto der)
        {
            PmSystemDto? systemPowerMeter = der.SystemPowerMeters.FirstOrDefault(device => device.Core?.ActivePower.HasValue == true && device.Core?.CommunicationError == false);

            if (systemPowerMeter is null)
            {
                return null;
            }

            return new ActivePower(systemPowerMeter.Core!.ActivePower!.Value.Watts);
        }


        /// <summary>
        /// Gets the measured reactive power at the point of interconnection from the system power meter.
        /// </summary>
        /// <param name="der">The DER device data.</param>
        /// <returns>The measured reactive power or null if no system power meter reports it.</returns>
        private static ReactivePower? GetSystemReactivePower(DerDto der)
        {
            PmSystemDto? systemPowerMeter = der.SystemPowerMeters.FirstOrDefault(device => device.Core?.ReactivePower.HasValue == true && device.Core?.CommunicationError == false);

            if (systemPowerMeter is null)
            {
                return null;
            }

            return new ReactivePower(systemPowerMeter.Core!.ReactivePower!.Value.VoltAmperesReactive);
        }
    }
}
