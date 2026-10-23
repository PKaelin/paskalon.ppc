// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PowerControls.Dto.Powers;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Luna power control manager interface definition.
    /// </summary>
    public interface ILunaPowerControlManager
    {
        /// <summary>
        /// Get the latest received power targets of all power conversion systems (PCS).
        /// </summary>
        /// <returns>Snapshot of the latest power targets, one per PCS.</returns>
        IReadOnlyCollection<CurrentTargetsDto> GetTargets();


        /// <summary>
        /// Get the latest received power targets of a power conversion system (PCS).
        /// </summary>
        /// <param name="deviceId">Device ID of the PCS.</param>
        /// <returns>The latest power targets of the PCS, or null if no power targets were received for the PCS.</returns>
        CurrentTargetsDto? GetTarget(int deviceId);
    }
}
