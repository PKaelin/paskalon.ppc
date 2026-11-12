// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyStorages.Batteries;

namespace paskalON.Devices.Domain.PowerConversionSystems
{
    /// <summary>
    /// Calculates the active power range a PCS can be commanded to right now.
    /// </summary>
    /// <remarks>
    /// The range uses the sign convention of the setpoints: the minimum (&lt;= 0) is the charge limit, the maximum (&gt;= 0) the discharge/production limit.
    /// A PCS reports one available active power value only. A battery PCS reports the limit of the current flow direction:
    /// a negative value is the charge limit, a positive value (or 0) the discharge limit.
    /// The other direction cannot be read from the device and is calculated from the nameplate of the PCS and its battery banks.
    /// Battery banks at the usable minimum state of charge cannot discharge, battery banks at the usable maximum cannot charge.
    /// </remarks>
    internal static class ActivePowerRangeCalculator
    {
        /// <summary>
        /// Calculates the active power range of a PCS.
        /// </summary>
        /// <param name="reportedAvailable">The available active power reported by the PCS in watts, null if unknown.</param>
        /// <param name="nameplateMaximumActivePower">The nameplate maximum active power of the PCS in watts.</param>
        /// <param name="derUnit">The DER unit of the PCS.</param>
        /// <returns>The minimum (charge) and maximum (discharge) active power in watts or null if the reported available power is unknown.</returns>
        public static (double Minimum, double Maximum)? Calculate(double? reportedAvailable, double nameplateMaximumActivePower, DerUnit derUnit)
        {
            ArgumentNullException.ThrowIfNull(derUnit);

            if (reportedAvailable.HasValue is false)
            {
                return null;
            }

            if (derUnit is DerBatteryStorageUnit batteryUnit)
            {
                return CalculateBattery(reportedAvailable.Value, nameplateMaximumActivePower, batteryUnit);
            }

            // Producing units (e.g. solar) cannot consume active power
            return (0, Math.Abs(reportedAvailable.Value));
        }


        /// <summary>
        /// Calculates the active power range of a battery PCS.
        /// </summary>
        /// <param name="reportedAvailable">The available active power reported by the PCS in watts.</param>
        /// <param name="nameplateMaximumActivePower">The nameplate maximum active power of the PCS in watts.</param>
        /// <param name="unit">The battery storage unit of the PCS.</param>
        /// <returns>The minimum (charge) and maximum (discharge) active power in watts.</returns>
        private static (double Minimum, double Maximum) CalculateBattery(double reportedAvailable, double nameplateMaximumActivePower, DerBatteryStorageUnit unit)
        {
            double dischargeRate = nameplateMaximumActivePower;
            double chargeRate = nameplateMaximumActivePower;

            // Without configured battery banks the state of charge is unknown to the PPC, the PCS and its BMS protect the battery
            if (HasConfiguredBanks(unit))
            {
                List<BatteryBankBase> banks = GetAvailableBanks(unit);
                dischargeRate = banks
                    .Where(IsDischargeable)
                    .Sum(bank => bank.NameplateMaximumDischargeRate);
                chargeRate = banks
                    .Where(IsChargeable)
                    .Sum(bank => bank.NameplateMaximumChargeRate);
            }

            double dischargeLimit = Math.Min(nameplateMaximumActivePower, dischargeRate);
            double chargeLimit = Math.Min(nameplateMaximumActivePower, chargeRate);

            // The reported value limits the direction it describes further (e.g. temperature or current derating)
            if (reportedAvailable < 0)
            {
                chargeLimit = Math.Min(chargeLimit, -reportedAvailable);
            }
            else
            {
                dischargeLimit = Math.Min(dischargeLimit, reportedAvailable);
            }

            // 0 - limit avoids a negative zero when the battery banks cannot charge
            return (0 - chargeLimit, dischargeLimit);
        }


        /// <summary>
        /// Checks whether at least one battery bank of the unit can discharge.
        /// </summary>
        /// <remarks>
        /// Same state of charge criterion as the active power range, so that the SOC safety of the PCS
        /// and the operating modes agree on when a battery is empty.
        /// </remarks>
        /// <param name="unit">The battery storage unit.</param>
        /// <returns>
        /// True if a connected battery bank is above its usable minimum state of charge
        /// or if no battery bank is configured (the state of charge is unknown to the PPC).
        /// </returns>
        public static bool CanDischarge(DerBatteryStorageUnit unit)
        {
            ArgumentNullException.ThrowIfNull(unit);

            return HasConfiguredBanks(unit) is false || GetAvailableBanks(unit).Any(IsDischargeable);
        }


        /// <summary>
        /// Checks whether at least one battery bank of the unit can charge.
        /// </summary>
        /// <remarks>
        /// Same state of charge criterion as the active power range, so that the SOC safety of the PCS
        /// and the operating modes agree on when a battery is full.
        /// </remarks>
        /// <param name="unit">The battery storage unit.</param>
        /// <returns>
        /// True if a connected battery bank is below its usable maximum state of charge
        /// or if no battery bank is configured (the state of charge is unknown to the PPC).
        /// </returns>
        public static bool CanCharge(DerBatteryStorageUnit unit)
        {
            ArgumentNullException.ThrowIfNull(unit);

            return HasConfiguredBanks(unit) is false || GetAvailableBanks(unit).Any(IsChargeable);
        }


        /// <summary>
        /// Checks whether battery banks are configured for the unit.
        /// </summary>
        /// <remarks>
        /// Configured but unavailable battery banks (disconnected, fault, unknown state of charge) block charging and discharging (fail-safe).
        /// Without configured battery banks the PCS is integrated without its battery management system and the SOC limits do not apply.
        /// </remarks>
        /// <param name="unit">The battery storage unit.</param>
        /// <returns>True if at least one battery bank is configured.</returns>
        private static bool HasConfiguredBanks(DerBatteryStorageUnit unit)
        {
            return unit.BatteryBanks.Count > 0;
        }


        /// <summary>
        /// Gets the battery banks that are connected (or in standby) and report their usable state of charge.
        /// </summary>
        /// <param name="unit">The battery storage unit.</param>
        /// <returns>The available battery banks.</returns>
        private static List<BatteryBankBase> GetAvailableBanks(DerBatteryStorageUnit unit)
        {
            return unit.BatteryBanks
                .Where(bank => (bank.State == BatteryBankState.Connected || bank.State == BatteryBankState.Standby) && bank.UsableStateOfCharge.HasValue)
                .ToList();
        }


        /// <summary>
        /// Checks whether the battery bank is above its usable minimum state of charge (usable SOC 0).
        /// </summary>
        /// <param name="bank">An available battery bank.</param>
        /// <returns>True if the battery bank can discharge.</returns>
        private static bool IsDischargeable(BatteryBankBase bank)
        {
            return bank.UsableStateOfCharge!.Value > 0;
        }


        /// <summary>
        /// Checks whether the battery bank is below its usable maximum state of charge (usable SOC = absolute maximum).
        /// </summary>
        /// <param name="bank">An available battery bank.</param>
        /// <returns>True if the battery bank can charge.</returns>
        private static bool IsChargeable(BatteryBankBase bank)
        {
            return bank.UsableStateOfCharge!.Value < bank.AbsoluteMaximumStateOfCharge;
        }
    }
}
