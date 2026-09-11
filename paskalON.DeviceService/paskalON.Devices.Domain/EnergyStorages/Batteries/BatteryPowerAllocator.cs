// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.EnergyStorages.Batteries
{
    /// <summary>
    /// Allocates one PCS output across the battery banks in a storage unit.
    /// </summary>
    internal sealed class BatteryPowerAllocator
    {
        /// <summary>
        /// Allocates active power to the battery banks in a unit.
        /// </summary>
        /// <param name="unit">Storage unit to update.</param>
        public void Allocate(DerBatteryStorageUnit unit)
        {
            ArgumentNullException.ThrowIfNull(unit);

            double totalPower = unit.PowerConversionSystem?.ActivePowerValue ?? 0;
            IReadOnlyCollection<BatteryBankBase> banks = unit.BatteryBanks;

            if (banks.Count == 0)
            {
                return;
            }

            List<(BatteryBankBase Bank, double Capacity)> available = banks
                .Where(b => b.IsInMaintenanceMode is false &&
                    (b.State == BatteryBankState.Connected || b.State == BatteryBankState.Standby) && b.StateOfCharge.HasValue)
                .Select(bank => (bank, Capacity: GetAvailablePower(bank, totalPower)))
                .Where(item => item.Capacity > 0)
                .ToList();

            double totalCapacity = available.Sum(item => item.Capacity);

            if (totalCapacity <= 0)
            {
                foreach ((BatteryBankBase bank, double capacity) in available)
                {
                    bank.AllocatedActivePowerValue = 0;
                }

                return;
            }

            foreach ((BatteryBankBase bank, double capacity) in available)
            {
                bank.AllocatedActivePowerValue = totalPower * capacity / totalCapacity;
            }
        }



        /// <summary>
        /// Gets the capacity of the battery bank using state of charge and nameplate definitions.
        /// </summary>
        /// <param name="bank">The battery bank.</param>
        /// <param name="totalPower">The total power to allocate.</param>
        /// <returns>The available power.</returns>
        private double GetAvailablePower(BatteryBankBase bank, double totalPower)
        {
            double stateOfCharge = Math.Clamp(bank.StateOfCharge!.Value, 0, 100);

            if (totalPower > 0)
            {
                double dischargeRange = 100 - bank.AbsoluteMinimumStateOfCharge;

                return dischargeRange <= 0 ? 0 : bank.NameplateMaximumDischargeRate *
                    Math.Clamp((stateOfCharge - bank.AbsoluteMinimumStateOfCharge) / dischargeRange, 0, 1);
            }

            double chargeRange = bank.AbsoluteMaximumStateOfCharge;

            return chargeRange <= 0 ? 0 : bank.NameplateMaximumChargeRate *
                Math.Clamp((bank.AbsoluteMaximumStateOfCharge - stateOfCharge) / chargeRange, 0, 1);
        }
    }
}