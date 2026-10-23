// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Dto.Powers;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Luna operating mode manager interface definition.
    /// </summary>
    public interface ILunaOperatingModeManager
    {
        /// <summary>
        /// Get the latest received system power setpoints.
        /// </summary>
        /// <returns>The latest system power setpoints, zero until the first setpoints are received.</returns>
        CurrentSetpointsDto GetSetpoints();


        /// <summary>
        /// Get the latest received power setpoints of all DER units.
        /// </summary>
        /// <returns>Snapshot of the latest DER unit power setpoints, one per DER unit.</returns>
        IReadOnlyCollection<CurrentDerUnitSetpointsDto> GetDerUnitSetpoints();


        /// <summary>
        /// Get the latest received power setpoints of a DER unit.
        /// </summary>
        /// <param name="derUnitName">Name of the DER unit.</param>
        /// <returns>The latest power setpoints of the DER unit, or null if no setpoints were received for the DER unit.</returns>
        CurrentDerUnitSetpointsDto? GetDerUnitSetpoints(string derUnitName);
    }
}
