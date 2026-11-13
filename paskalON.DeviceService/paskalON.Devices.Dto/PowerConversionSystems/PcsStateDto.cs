// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.PowerConversionSystems
{
    /// <summary>
    /// PCS states published by the device service.
    /// </summary>
    /// <remarks>
    /// Mirrors PcsState (paskalON.Devices.Domain) with the same names and numbers, mapped in the application layer.
    /// </remarks>
    public enum PcsStateDto
    {
        /// <summary>
        /// The current state of the PCS is not determined or is undefined.
        /// </summary>
        Unknown = 0,


        /// <summary>
        /// The PCS has stopped or is off.
        /// </summary>
        Stopped = 1,


        /// <summary>
        /// The PCS is starting and initializing.
        /// </summary>
        Starting = 2,


        /// <summary>
        /// The PCS is started and ready to receive targets.
        /// </summary>
        Started = 3,


        /// <summary>
        /// The PCS has been commanded to stop and is stopping.
        /// </summary>
        Stopping = 4,


        /// <summary>
        /// The PCS is sleeping, a kind of standby that needs more time to get into the started state.
        /// </summary>
        Sleeping = 5,


        /// <summary>
        /// The PCS is entering standby.
        /// </summary>
        EnteringStandby = 6,


        /// <summary>
        /// The PCS is in standby.
        /// </summary>
        Standby = 7,


        /// <summary>
        /// The PCS is exiting standby.
        /// </summary>
        ExitingStandby = 8,


        /// <summary>
        /// The PCS is in night mode.
        /// </summary>
        NightMode = 9,


        /// <summary>
        /// The PCS encountered an error or abnormal condition that requires attention.
        /// </summary>
        Fault = 99
    }
}
