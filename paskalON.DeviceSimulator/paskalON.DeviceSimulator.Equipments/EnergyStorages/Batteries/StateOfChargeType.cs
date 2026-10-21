// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries
{
    /// <summary>
    /// State of charge window a battery bank reports in its state of charge register.
    /// </summary>
    public enum StateOfChargeType
    {
        /// <summary>
        /// Usable (manufacturer window) state of charge.
        /// </summary>
        Usable = 0,

        /// <summary>
        /// Absolute (physical cells) state of charge.
        /// </summary>
        Absolute = 1,

        /// <summary>
        /// Preferred (operator window) state of charge.
        /// </summary>
        Preferred = 2,
    }
}
