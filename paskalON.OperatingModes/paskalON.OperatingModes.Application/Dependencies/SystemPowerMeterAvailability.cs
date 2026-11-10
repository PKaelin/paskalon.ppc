// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.Meters.PowerMeters.Concretes;

namespace paskalON.OperatingModes.Application.Dependencies
{
    /// <summary>
    /// Determines whether the measurement at the point of interconnection (system power meter) is available.
    /// </summary>
    public static class SystemPowerMeterAvailability
    {
        /// <summary>
        /// Gets whether a system power meter delivers valid data.
        /// </summary>
        /// <param name="der">The DER device data.</param>
        /// <returns>
        /// True if at least one system power meter has no communication error and reports the active and reactive power,
        /// or if no system power meter is configured (there is nothing to lose). False if no system power meter delivers valid data.
        /// </returns>
        public static bool IsAvailable(DerDto der)
        {
            ArgumentNullException.ThrowIfNull(der);

            if (der.SystemPowerMeters.Count == 0)
            {
                return true;
            }

            return der.SystemPowerMeters.Any(IsValid);
        }


        /// <summary>
        /// Gets whether a system power meter delivers valid data.
        /// </summary>
        /// <param name="systemPowerMeter">The system power meter.</param>
        /// <returns>True if the meter has received data without communication error that contains the active and reactive power.</returns>
        private static bool IsValid(PmSystemDto systemPowerMeter)
        {
            PmSystemCoreDto? core = systemPowerMeter.Core;

            return (core is not null) && (core.CommunicationError is false) && core.ActivePower.HasValue && core.ReactivePower.HasValue;
        }
    }
}
