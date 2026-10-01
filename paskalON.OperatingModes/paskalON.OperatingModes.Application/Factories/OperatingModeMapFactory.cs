// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.OpenModes;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.OpenModes.VoltageReactives;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Application.Factories
{
    /// <summary>
    /// Factory class for creating operating mode maps based on configurations and DER information.
    /// </summary>
    public class OperatingModeMapFactory : IOperatingModeMapFactory
    {
        /// <inheritdoc/>
        public OperatingModeBaseMap? Create(OperatingModeBaseConfig config, DerDto der)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(der);

            if (config is MaintenanceModeConfig)
            {
                DerUnitDto? unit = der.DerGroups.SelectMany(g => g.DerCircuits).SelectMany(c => c.DerUnits)
                    .FirstOrDefault(u => u.Name == ((MaintenanceModeConfig)config).DerUnitName);

                if (unit == null)
                {
                    throw new InvalidOperationException($"DER unit '{der.Name}' not found in DER '{der.Name}'.");
                }

                return new MaintenanceModeMap
                {
                    AvailableActivePower = () => GetAvailableActivePower(der),
                    AvailableReactivePower = () => GetAvailableReactivePower(der),
                    DerUnitName = unit.Name
                };
            }

            if (config is ActivePowerFixedModeConfig)
            {
                return new ActivePowerFixedModeMap
                {
                    AvailableActivePower = () => GetAvailableActivePower(der),
                    AvailableReactivePower = () => GetAvailableReactivePower(der)
                };
            }

            if (config is ReactivePowerFixedModeConfig)
            {
                return new ReactivePowerFixedModeMap
                {
                    AvailableActivePower = () => GetAvailableActivePower(der),
                    AvailableReactivePower = () => GetAvailableReactivePower(der)
                };
            }

            return null;
        }


        /// <summary>
        /// Calculates the available active power for the given DER by summing the active power of all battery storage and solar units.
        /// </summary>
        /// <param name="der">The DER for which to calculate the available active power.</param>
        /// <returns>The available active power.</returns>
        private ActivePower GetAvailableActivePower(DerDto der)
        {
            double totalActivePower = der.DerGroups.SelectMany(g => g.DerCircuits).SelectMany(c => c.DerUnits).OfType<DerBatteryStorageUnitDto>().Where(p => p.PowerConversionSystem.Core?.ActivePower.HasValue == true).Sum(p => p.PowerConversionSystem.Core!.ActivePower!.Value.Watts);
            totalActivePower += der.DerGroups.SelectMany(g => g.DerCircuits).SelectMany(c => c.DerUnits).OfType<DerSolarUnitDto>().Where(p => p.PowerConversionSystem.Core?.ActivePower.HasValue == true).Sum(p => p.PowerConversionSystem.Core!.ActivePower!.Value.Watts);

            return new ActivePower(totalActivePower);
        }


        /// <summary>
        /// Calculates the available reactive power for the given DER by summing the reactive power of all battery storage units.
        /// </summary>
        /// <param name="der">The DER for which to calculate the available reactive power.</param>
        /// <returns>The available reactive power.</returns>
        private ReactivePower GetAvailableReactivePower(DerDto der)
        {
            double totalReactivePower = der.DerGroups.SelectMany(g => g.DerCircuits).SelectMany(c => c.DerUnits).OfType<DerBatteryStorageUnitDto>().Where(p => p.PowerConversionSystem.Core?.ReactivePower.HasValue == true).Sum(p => p.PowerConversionSystem.Core!.ReactivePower!.Value.VoltAmperesReactive);

            return new ReactivePower(totalReactivePower);
        }


        /// <summary>
        /// Gets the total active power of the system power meters.
        /// </summary>
        /// <param name="der">The DER for which to calculate the total active power.</param>
        /// <returns>The total active power of the system.</returns>
        private ActivePower? GetSystemActivePower(DerDto der)
        {
            IEnumerable<PmSystemDto> systemPowerMeters = der.SystemPowerMeters.Where(device => device.Core?.ActivePower.HasValue == true && device.Core?.CommunicationError == false);

            if (systemPowerMeters.Count() == 0)
            {
                return null;
            }

            return new ActivePower(systemPowerMeters.First().Core!.ActivePower!.Value.Watts);
        }


        /// <summary>
        /// Gets the total reactive power of the system power meters.
        /// </summary>
        /// <param name="der">The DER for which to calculate the total reactive power.</param>
        /// <returns>The total reactive power of the system.</returns>
        private ReactivePower? GetSystemReactivePower(DerDto der)
        {
            IEnumerable<PmSystemDto> systemPowerMeters = der.SystemPowerMeters.Where(device => device.Core?.ReactivePower.HasValue == true && device.Core?.CommunicationError == false);

            if (systemPowerMeters.Count() == 0)
            {
                return null;
            }

            return new ReactivePower(systemPowerMeters.First().Core!.ReactivePower!.Value.VoltAmperesReactive);
        }
    }
}
