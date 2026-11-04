// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.EnergyResources.Solars
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Event argument class for solar panel state changed events.
    /// </summary>
    public class SolarPanelStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Solar panel state.
        /// </summary>
        public SolarPanelState State { get; private set; }


        /// <summary>
        /// Constructor of <see cref="SolarPanelStateChangedEventArgs"/>.
        /// </summary>
        /// <param name="state">The solar panel state.</param>
        public SolarPanelStateChangedEventArgs(SolarPanelState state)
        {
            State = state;
        }
    }
}