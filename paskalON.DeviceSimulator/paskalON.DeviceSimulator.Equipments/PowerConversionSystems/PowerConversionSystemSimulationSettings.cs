// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.PowerConversionSystems;

namespace paskalON.DeviceSimulator.Equipments.PowerConversionSystems
{
    /// <summary>
    /// Nameplate settings of a simulated PCS.
    /// </summary>
    public sealed record PowerConversionSystemSimulationSettings
    {
        /// <summary>
        /// Device ID of the PCS.
        /// </summary>
        public required int DeviceId { get; init; }


        /// <summary>
        /// Name of the PCS.
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// Nameplate maximum active power in watts.
        /// </summary>
        public required double NameplateMaximumActivePower { get; init; }


        /// <summary>
        /// Nameplate maximum reactive power in volt-amperes reactive.
        /// </summary>
        public required double NameplateMaximumReactivePower { get; init; }


        /// <summary>
        /// Nameplate maximum apparent power in volt-amperes. 0 when not limited by the apparent power.
        /// </summary>
        public double NameplateMaximumApparentPower { get; init; }


        /// <summary>
        /// Nameplate maximum AC current in amperes. 0 when unknown.
        /// </summary>
        public double NameplateMaximumACCurrent { get; init; }


        /// <summary>
        /// Nominal AC line to line voltage at the PCS terminals in volts, derived from the nameplate apparent power and AC current.
        /// </summary>
        /// <remarks>
        /// V = S / (√3 · I). 0 when the nameplate apparent power or AC current is unknown.
        /// </remarks>
        public double NominalACVoltage
        {
            get
            {
                if (NameplateMaximumApparentPower <= 0 || NameplateMaximumACCurrent <= 0)
                {
                    return 0;
                }

                return NameplateMaximumApparentPower / (Math.Sqrt(3) * NameplateMaximumACCurrent);
            }
        }


        /// <summary>
        /// Creates the settings from a PCS device.
        /// </summary>
        /// <param name="device">PCS device.</param>
        /// <returns>The PCS simulation settings.</returns>
        public static PowerConversionSystemSimulationSettings FromDevice(PowerConversionSystemBase device)
        {
            ArgumentNullException.ThrowIfNull(device);

            return new PowerConversionSystemSimulationSettings
            {
                DeviceId = device.DeviceId,
                Name = device.Name,
                NameplateMaximumActivePower = Math.Abs(device.NameplateMaximumActivePower.Watts),
                NameplateMaximumReactivePower = Math.Abs(device.NameplateMaximumReactivePower.VoltAmperesReactive),
                NameplateMaximumApparentPower = Math.Abs(device.NameplateMaximumApparentPower.VoltAmperes),
                NameplateMaximumACCurrent = Math.Abs(device.NameplateMaximumACCurrent)
            };
        }
    }
}
