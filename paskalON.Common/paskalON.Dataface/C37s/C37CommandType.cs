// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    /// <summary>
    /// Enum for byte masks used to interpret the command field of the command frame.
    /// </summary>
    public enum C37CommandType
    {
        /// <summary>
        /// Indicate the PMU or server should stop sending data.
        /// </summary>
        TurnOffTransmission = 1,
        /// <summary>
        /// Indicate to the PMU or server should be sending data.
        /// </summary>
        TurnOnTransmission = 2,
        /// <summary>
        /// Request to the PMU/server for the header frame.
        /// </summary>
        SendHeaderFrame = 3,
        /// <summary>
        ///  Request to the PMU/server for the Config1 Frame.
        /// </summary>
        SendConfigFrame1 = 4,
        /// <summary>
        ///  Request to the PMU/server for the Config2 Frame.     
        /// </summary>
        SendConfigFrame2 = 5,
        /// <summary>
        ///  Request to the PMU/server for the Config3 Frame.
        /// </summary>
        SendConfigFrame3 = 6
    }
}
