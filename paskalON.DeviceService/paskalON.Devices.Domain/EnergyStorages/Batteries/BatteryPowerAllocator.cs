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

            double totalPower = unit.PowerConversionSystem?.ActivePower?.Watts ?? 0;
            IReadOnlyCollection<BatteryBankBase> banks = unit.BatteryBanks;

            if (banks.Count == 0)
            {
                return;
            }

            List<(BatteryBankBase Bank, double Weight)> available = banks
                .Where(b => b.IsInMaintenanceMode is false &&
                    (b.State == BatteryBankState.Connected || b.State == BatteryBankState.Standby) && b.StateOfCharge.HasValue)
                .Select(bank => (bank, Weight: bank.StateOfCharge ?? 0))
                .Where(item => item.Weight > 0)
                .ToList();

            double totalWeight = available.Sum(item => item.Weight);

            if (available.Count > 0)
            {
                // Allocate simple distribution. At this point do not realocated when nameplate hits one of the banks
                foreach ((BatteryBankBase bank, double Weight) in available)
                {
                    // Discharge
                    if (totalPower > 0)
                    {
                        bank.AllocatedActivePowerValue = Math.Min(totalPower * Weight / totalWeight, bank.NameplateMaximumDischargeRate);
                    }
                    // Charge
                    else if (totalPower < 0)
                    {
                        bank.AllocatedActivePowerValue = Math.Max(totalPower * Weight / totalWeight, bank.NameplateMaximumChargeRate * -1);
                    }
                    else
                    {
                        bank.AllocatedActivePowerValue = 0;
                    }
                }
            }
            else
            {
                foreach (BatteryBankBase bank in banks)
                {
                    bank.AllocatedActivePowerValue = 0;
                }
            }
        }
    }
}