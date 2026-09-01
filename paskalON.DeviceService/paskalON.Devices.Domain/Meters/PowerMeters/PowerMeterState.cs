// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.Meters.PowerMeters
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Power meter states.
    /// </summary>
    /// <remarks>
    /// Underlying state have to be mapped to these states.
    /// </remarks>
    public enum PowerMeterState
    {
        /// <summary>
        /// Unknown means that the power meter's current state is not determined or is in an undefined state.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Disconnected means that the internal mechanical relay (or contactor) has opened, cutting off the flow of electricity to the property or load.
        /// Power is still coming to the meter from the grid, but the meter is actively preventing it from passing through
        /// </summary>
        Disconnected = 1,
        /// <summary>
        /// Connecting means that the power meter is in the process of establishing a connection.
        /// </summary>
        Connecting = 2,
        /// <summary>
        /// Standby means the power meter is connected and fully operational. 
        /// </summary>
        Connected = 3,
        /// <summary>
        /// Disconnecting means that the power meter is in the process of disconnecting.
        /// </summary>
        Disconnecting = 4
    }
}
