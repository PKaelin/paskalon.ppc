// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.PhysicalUnits.Percentages
{
    public static class StateOfChargeCalculator
    {
        /// <summary>
        /// Get the absolute State of Charge relative to the preferred State of Charge.
        /// </summary>
        /// <param name="preferredSoc">Preferred SOC.</param>
        /// <param name="preferredMinimumStateOfCharge">Preferred Minimum SOC.</param>
        /// <param name="preferredMaximumStateOfCharge">Preferred Maximum SOC.</param>
        /// <param name="absoluteMaximumStateOfCharge">Absolute Maximum SOC./param>
        /// <returns>Return the relative absolute State of Charge.</returns>
        /// <remarks>
        /// The preferred SOC is what the system/user sees so it should be always between 0% and 100%
        /// The absolute is the physical boundary which should never be reached.
        /// </remarks>
        public static double GetAbsoluteStateOfChargeFromPreferred(double preferredSoc, double preferredMinimumStateOfCharge,
            double preferredMaximumStateOfCharge, double absoluteMaximumStateOfCharge)
        {
            return preferredMinimumStateOfCharge + (preferredSoc / absoluteMaximumStateOfCharge) *
                    (preferredMaximumStateOfCharge - preferredMinimumStateOfCharge);
        }



        /// <summary>
        /// Get the absolute State of Charge relative to the usable State of Charge.
        /// </summary>
        /// <param name="usableSoc">Usable SOC.</param>
        /// <param name="usableMinimumStateOfCharge">Usable Minimum SOC.</param>
        /// <param name="usableMaximumStateOfCharge">Usable Maximum SOC.</param>
        /// <param name="absoluteMaximumStateOfCharge">Absolute Maximum SOC./param>
        /// <returns>Return the relative absolute State of Charge.</returns>
        public static double? GetAbsoluteStateOfChargeFromUsable(double? usableSoc, double usableMinimumStateOfCharge,
            double usableMaximumStateOfCharge, double absoluteMaximumStateOfCharge)
        {
            if (usableSoc == null)
            {
                return null;
            }

            return usableMinimumStateOfCharge + usableSoc * (usableMaximumStateOfCharge - usableMinimumStateOfCharge) / absoluteMaximumStateOfCharge;
        }



        /// <summary>
        /// Get the usable State of Charge relative to the absolute State of Charge.
        /// </summary>
        /// <param name="absoluteSoc">Absolute SOC.</param>
        /// <param name="usableMinimumStateOfCharge">Usable minimum SOC.</param>
        /// <param name="usableMaximumStateOfCharge">Usable maximum SOC.</param>
        /// <param name="absoluteMaximumStateOfCharge">Absolute maximum SOC.</param>
        /// <returns>Returns the usable State of Charge.</returns>
        /// The usable SOC is either the same as preferred SOC or outside the preferred SOC but within the absolute SOC.
        /// The absolute is the physical boundary which should never be reached. 
        public static double GetUsableStateOfChargeFromAbsolute(double absoluteSoc, double usableMinimumStateOfCharge,
            double usableMaximumStateOfCharge, double absoluteMaximumStateOfCharge)
        {
            return (absoluteSoc - usableMinimumStateOfCharge) /
                (usableMaximumStateOfCharge - usableMinimumStateOfCharge) * absoluteMaximumStateOfCharge;
        }



        /// <summary>
        /// Get the preferred State of Charge relative to the absolute State of Charge.
        /// </summary>
        /// <param name="absoluteSoc">Absolute SOC.</param>
        /// <param name="preferredMinimumStateOfCharge">Preferred Minimum SOC.</param>
        /// <param name="preferredMaximumStateOfCharge">Preferred Maximum SOC.</param>
        /// <param name="absoluteMaximumStateOfCharge">Absolute Maximum SOC./param>
        /// <returns>Return the relative preferred State of Charge.</returns>
        public static double? GetPreferredStateOfChargeFromAbsolute(double? absoluteSoc, double preferredMinimumStateOfCharge,
            double preferredMaximumStateOfCharge, double absoluteMaximumStateOfCharge)
        {
            if (absoluteSoc == null)
            {
                return null;
            }

            return (absoluteSoc - preferredMinimumStateOfCharge) / (preferredMaximumStateOfCharge - preferredMinimumStateOfCharge) * absoluteMaximumStateOfCharge;
        }


        /// <summary>
        /// Gets the usable capacity from the nameplate capacity.
        /// </summary>
        /// <param name="nameplateCapacity">The nameplate capacity.</param>
        /// <param name="usableMinimumStateOfCharge">Usable minimum SOC.</param>
        /// <param name="usableMaximumStateOfCharge">Usable maximum SOC.</param>
        /// <param name="absoluteMinimumStateOfCharge">Absolute minimum SOC.</param>
        /// <param name="absoluteMaximumStateOfCharge">Absolute maximum SOC.</param>
        /// <returns>Returns the usable capacity.</returns>
        public static double GetUsableCapacity(double nameplateCapacity, double usableMinimumStateOfCharge,
            double usableMaximumStateOfCharge, double absoluteMinimumStateOfCharge, double absoluteMaximumStateOfCharge)
        {
            return nameplateCapacity * (usableMaximumStateOfCharge - usableMinimumStateOfCharge) /
                (absoluteMaximumStateOfCharge - absoluteMinimumStateOfCharge);
        }
    }
}
