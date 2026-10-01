// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives
{
    /// <summary>
    /// Input mapping class for frequency droop mode.
    /// </summary>
    public class FrequencyDroopModeMap : OperatingModeBaseMap
    {
        /// <summary>
        /// Frequency at the POI map.
        /// </summary>
        public required Func<double?> FrequencyAtPoi { get; set; }
    }
}
