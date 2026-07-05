// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.Meters.PowerMeters
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Event argument class for power meter state changed events.
    /// </summary>
    public class PowerMeterStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// PowerMeter state.
        /// </summary>
        public PowerMeterState State { get; private set; }


        /// <summary>
        /// Constructor of <see cref="PowerMeterStateChangedEventArgs"/>.
        /// </summary>
        /// <param name="state">The power meter state.</param>
        public PowerMeterStateChangedEventArgs(PowerMeterState state)
        {
            State = state;
        }
    }
}
