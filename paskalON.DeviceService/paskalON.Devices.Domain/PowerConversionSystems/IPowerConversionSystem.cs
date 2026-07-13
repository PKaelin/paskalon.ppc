// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.PowerConversionSystems
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Power conversion system interface for the instances that communicate with the device.
    /// </summary>
    public interface IPowerConversionSystem : IDevice
    {
        /// <summary>
        /// Starts the power conversion system.
        /// </summary>
        Task StartAsync();


        /// <summary>
        /// Stops the power conversion system.
        /// </summary>
        Task StopAsync();


        /// <summary>
        /// Puts the power conversion system in standby mode.
        /// </summary>
        /// <remarks>
        /// The standby mode shall have a minimum active power target configured in the PCS.
        /// This could be required for PCSs that need a minimum active power to be able to switch on properly.
        /// If not standby active power is provided, the PCS will use the minimum active power target configured in the PCS.
        /// </remarks>
        Task StandbyAsync(double? standbyActivePower = null);


        /// <summary>
        /// Sets the active power target.
        /// </summary>
        /// <param name="value">Active power value (Watts).</param>
        Task SetActivePowerTargetAsync(double? value);


        /// <summary>
        /// Sets the reactive power target.
        /// </summary>
        /// <param name="value">Reactive power value (VArs).</param>
        Task SetReactivePowerTargetAsync(double? value);
    }
}
