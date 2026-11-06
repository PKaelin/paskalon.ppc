// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs.Ppcs;

namespace paskalON.Alarms.Domain.Evaluations
{
    /// <summary>
    /// Key of the published data in the stateless store.
    /// </summary>
    /// <remarks>
    /// The key identifies the publisher of the data, e.g. PcsAlarm and the device id of the PCS.
    /// </remarks>
    public readonly record struct PpcAlarmKey
    {
        /// <summary>
        /// Type of the PPC alarm (the publishing device type).
        /// </summary>
        public required PpcAlarmType PpcAlarmType { get; init; }


        /// <summary>
        /// Unique identifier of the publishing device.
        /// </summary>
        /// <remarks>
        /// Null means the data is not published by a device, e.g. system data.
        /// </remarks>
        public int? DeviceId { get; init; }
    }
}
