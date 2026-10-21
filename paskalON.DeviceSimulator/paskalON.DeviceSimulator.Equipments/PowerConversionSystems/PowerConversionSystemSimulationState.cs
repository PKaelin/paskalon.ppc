// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Equipments.PowerConversionSystems
{
    /// <summary>
    /// Vendor independent state of a simulated PCS.
    /// </summary>
    public enum PowerConversionSystemSimulationState
    {
        /// <summary>
        /// PCS is stopped, no power is converted and the AC breaker is open.
        /// </summary>
        Off = 0,

        /// <summary>
        /// PCS is started and follows its active and reactive power setpoints.
        /// </summary>
        On = 1,

        /// <summary>
        /// PCS is in standby and follows its (standby) active and reactive power setpoints.
        /// </summary>
        Standby = 2,

        /// <summary>
        /// PCS is faulted, no power is converted and the AC breaker is open.
        /// </summary>
        /// <remarks>
        /// The PCS leaves the fault state with the next start, stop or standby command of the controller.
        /// </remarks>
        Fault = 3,
    }
}
