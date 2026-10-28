// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Luna.Dto.Systems
{
    /// <summary>
    /// System nameplate ratings data transfer object (DTO).
    /// </summary>
    public record SystemNameplateDto
    {
        /// <summary>
        /// Maximum active power of the system nameplate rating in watts (W).
        /// </summary>
        public double MaximumActivePowerWatts { get; init; }


        /// <summary>
        /// Minimum active power of the system nameplate rating in watts (W).
        /// </summary>
        public double MinimumActivePowerWatts { get; init; }


        /// <summary>
        /// Maximum reactive power of the system nameplate rating in volt-amperes reactive (var).
        /// </summary>
        public double MaximumReactivePowerVars { get; init; }


        /// <summary>
        /// Minimum reactive power of the system nameplate rating in volt-amperes reactive (var).
        /// </summary>
        public double MinimumReactivePowerVars { get; init; }
    }
}
