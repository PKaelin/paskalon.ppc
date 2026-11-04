// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.PowerConversionSystems
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Event argument class for power conversion system (PCS) state changed events.
    /// </summary>
    public class PcsStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// PCS state.
        /// </summary>
        public PcsState State { get; private set; }


        /// <summary>
        /// Constructor of <see cref="PcsStateChangedEventArgs"/>.
        /// </summary>
        /// <param name="state">The power conversion system (PCS) state.</param>
        public PcsStateChangedEventArgs(PcsState state)
        {
            State = state;
        }
    }
}
