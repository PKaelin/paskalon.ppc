// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.DeviceSimulator.Equipments.Simulations;

namespace paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries
{
    /// <summary>
    /// Nameplate and simulation settings of a simulated battery bank.
    /// </summary>
    public sealed record BatteryBankSimulationSettings
    {
        /// <summary>
        /// Custom configuration key of the pre-charge delay in seconds.
        /// </summary>
        public const string PreChargeDelaySecondsKey = "SimPreChargeDelaySeconds";


        /// <summary>
        /// Device ID of the battery bank.
        /// </summary>
        public required int DeviceId { get; init; }


        /// <summary>
        /// Name of the battery bank.
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// Nameplate capacity in watt-hours.
        /// </summary>
        public required double NameplateCapacity { get; init; }


        /// <summary>
        /// Nameplate maximum charge rate in watts (positive value).
        /// </summary>
        public required double NameplateMaximumChargeRate { get; init; }


        /// <summary>
        /// Nameplate maximum discharge rate in watts (positive value).
        /// </summary>
        public required double NameplateMaximumDischargeRate { get; init; }


        /// <summary>
        /// Absolute minimum state of charge in percent.
        /// </summary>
        public double AbsoluteMinimumStateOfCharge { get; init; } = 0;


        /// <summary>
        /// Absolute maximum state of charge in percent.
        /// </summary>
        public double AbsoluteMaximumStateOfCharge { get; init; } = 100;


        /// <summary>
        /// Usable minimum state of charge in percent of the absolute state of charge.
        /// </summary>
        public double UsableMinimumStateOfCharge { get; init; } = 10;


        /// <summary>
        /// Usable maximum state of charge in percent of the absolute state of charge.
        /// </summary>
        public double UsableMaximumStateOfCharge { get; init; } = 90;


        /// <summary>
        /// Preferred minimum state of charge in percent of the absolute state of charge.
        /// </summary>
        public double PreferredMinimumStateOfCharge { get; init; } = 20;


        /// <summary>
        /// Preferred maximum state of charge in percent of the absolute state of charge.
        /// </summary>
        public double PreferredMaximumStateOfCharge { get; init; } = 80;


        /// <summary>
        /// DC voltage at 0% absolute state of charge in volts.
        /// </summary>
        public double MinimumDcVoltage { get; init; }


        /// <summary>
        /// DC voltage at 100% absolute state of charge in volts.
        /// </summary>
        public double MaximumDcVoltage { get; init; }


        /// <summary>
        /// Delay between the connect command and the connected state.
        /// </summary>
        public TimeSpan PreChargeDelay { get; init; } = TimeSpan.Zero;


        /// <summary>
        /// Creates the settings from a battery bank and its simulator custom configurations.
        /// </summary>
        /// <param name="device">Battery bank device.</param>
        /// <param name="customs">Simulator custom configurations (key/value).</param>
        /// <returns>The battery bank simulation settings.</returns>
        public static BatteryBankSimulationSettings FromDevice(BatteryBankBase device, IReadOnlyDictionary<string, string> customs)
        {
            ArgumentNullException.ThrowIfNull(device);
            ArgumentNullException.ThrowIfNull(customs);

            return new BatteryBankSimulationSettings
            {
                DeviceId = device.DeviceId,
                Name = device.Name,
                NameplateCapacity = device.NameplateCapacity,
                NameplateMaximumChargeRate = Math.Abs(device.NameplateMaximumChargeRate),
                NameplateMaximumDischargeRate = Math.Abs(device.NameplateMaximumDischargeRate),
                AbsoluteMinimumStateOfCharge = device.AbsoluteMinimumStateOfCharge,
                AbsoluteMaximumStateOfCharge = device.AbsoluteMaximumStateOfCharge,
                UsableMinimumStateOfCharge = device.UsableMinimumStateOfCharge,
                UsableMaximumStateOfCharge = device.UsableMaximumStateOfCharge,
                PreferredMinimumStateOfCharge = device.PreferredMinimumStateOfCharge,
                PreferredMaximumStateOfCharge = device.PreferredMaximumStateOfCharge,
                MinimumDcVoltage = device.MinimumDcVoltage,
                MaximumDcVoltage = device.MaximumDcVoltage,
                PreChargeDelay = TimeSpan.FromSeconds(SimulationCustoms.GetDouble(customs, PreChargeDelaySecondsKey, 0))
            };
        }
    }
}
