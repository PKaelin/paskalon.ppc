// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Client;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;
using paskalON.Luna.Dto.Devices;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Luna device manager to get device information.
    /// </summary>
    public class LunaDeviceManager : ILunaDeviceManager
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<LunaDeviceManager> _logger;


        /// <summary>
        /// Device client to get DER configuration and to receive device DTOs from pub/sub.
        /// </summary>
        private readonly IDeviceClient _deviceClient;


        /// <summary>
        /// Constructor of <see cref="LunaDeviceManager"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="deviceClient">Device client to get DER configuration and to receive device DTOs from pub/sub.</param>
        public LunaDeviceManager(ILogger<LunaDeviceManager> logger, IDeviceClient deviceClient)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceClient);

            _logger = logger;
            _deviceClient = deviceClient;
        }


        /// <summary>
        /// Get the DER configuration DTOs
        /// </summary>
        /// <returns>The DER configuration DTO</returns>
        public DerDto GetDer()
        {
            return _deviceClient.Der;
        }


        /// <inheritdoc/>
        public SystemMeterSummaryDto? GetSystemMeterSummary()
        {
            DerDto der = _deviceClient.Der;
            PmSystemCoreDto? core = der.SystemPowerMeters
                .Select(meter => meter.Core)
                .FirstOrDefault(meterCore => meterCore is { CommunicationError: false, State: PowerMeterState.Connected });

            if (core == null)
            {
                return null;
            }

            return new SystemMeterSummaryDto
            {
                ActivePower = core.ActivePower ?? new ActivePower(0),
                ReactivePower = core.ReactivePower ?? new ReactivePower(0),
                StateOfCharge = GetAverageStateOfCharge(der),
                PowerFactor = core.PowerFactor ?? 0,
                Frequency = core.Frequency ?? 0
            };
        }


        /// <inheritdoc/>
        public PmSystemDto? GetSystemPowerMeter(int deviceId)
        {
            return _deviceClient.Der.SystemPowerMeters.FirstOrDefault(meter => meter.DeviceId == deviceId);
        }


        /// <inheritdoc/>
        public PmAuxiliaryDto? GetAuxiliaryPowerMeter(int deviceId)
        {
            return _deviceClient.Der.AuxiliaryPowerMeters.FirstOrDefault(meter => meter.DeviceId == deviceId);
        }


        /// <inheritdoc/>
        public PmExternalDto? GetExternalPowerMeter(int deviceId)
        {
            return _deviceClient.Der.ExternalPowerMeters.FirstOrDefault(meter => meter.DeviceId == deviceId);
        }


        /// <summary>
        /// Averages the state of charge of all battery banks that have no communication error and a state of charge.
        /// </summary>
        /// <param name="der">The DER configuration DTO.</param>
        /// <returns>The average state of charge in percent, or null if no battery bank has a state of charge.</returns>
        private double? GetAverageStateOfCharge(DerDto der)
        {
            List<double> stateOfCharges = der.DerGroups
                .SelectMany(group => group.DerCircuits)
                .SelectMany(circuit => circuit.DerUnits)
                .OfType<DerBatteryStorageUnitDto>()
                .SelectMany(unit => unit.BatteryBanks)
                .Select(batteryBank => batteryBank.Core)
                .OfType<BbCoreDto>()
                .Where(core => core.CommunicationError == false)
                .Select(core => core.StateOfCharge)
                .OfType<double>()
                .ToList();

            if (stateOfCharges.Count == 0)
            {
                return null;
            }

            return stateOfCharges.Average();
        }
    }
}
