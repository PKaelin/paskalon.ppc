// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries
{
    /// <summary>
    /// Splits the active power of a PCS across its battery banks in proportion to their usable capacity.
    /// </summary>
    /// <remarks>
    /// This is the physical split of the simulated DC bus and intentionally independent of the controller's
    /// battery power allocator. When a battery bank reaches its power limit, the remaining power is shared
    /// by the other battery banks (water filling).
    /// </remarks>
    public static class BatteryPowerSplitter
    {
        /// <summary>
        /// Power below this value in watts is treated as fully allocated.
        /// </summary>
        private const double Tolerance = 1e-6;


        /// <summary>
        /// Splits the active power across the candidates.
        /// </summary>
        /// <param name="activePower">Active power in watts (positive discharging, negative charging).</param>
        /// <param name="candidates">Weight (usable capacity) and power limit (positive value) of every battery bank.</param>
        /// <returns>The allocated active power of every candidate in the same order, with the sign of the active power.</returns>
        public static double[] Split(double activePower, IReadOnlyList<(double Weight, double Limit)> candidates)
        {
            ArgumentNullException.ThrowIfNull(candidates);

            double[] allocations = new double[candidates.Count];
            double remaining = Math.Abs(activePower);
            List<int> open = Enumerable.Range(0, candidates.Count)
                .Where(index => candidates[index].Weight > 0 && candidates[index].Limit > 0)
                .ToList();

            while (remaining > Tolerance && open.Count > 0)
            {
                double totalWeight = open.Sum(index => candidates[index].Weight);
                List<int> saturated = new List<int>();
                double allocated = 0;

                foreach (int index in open)
                {
                    double share = remaining * candidates[index].Weight / totalWeight;
                    double headroom = candidates[index].Limit - allocations[index];

                    if (share >= headroom)
                    {
                        share = headroom;
                        saturated.Add(index);
                    }

                    allocations[index] += share;
                    allocated += share;
                }

                remaining -= allocated;

                if (saturated.Count == 0)
                {
                    break;
                }

                open.RemoveAll(saturated.Contains);
            }

            double sign = activePower < 0 ? -1 : 1;

            return allocations.Select(allocation => allocation * sign).ToArray();
        }
    }
}
