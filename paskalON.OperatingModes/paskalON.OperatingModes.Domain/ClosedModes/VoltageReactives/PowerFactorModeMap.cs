// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.ClosedModes.VoltageReactives
{
    /// <summary>
    /// Input mapping class for power factor mode.
    /// </summary>
    public class PowerFactorModeMap : OperatingModeBaseMap
    {
        /// <summary>
        /// Gets or sets the power factor standard.
        /// </summary>
        public PowerFactorStandard PowerFactorStandard { get; set; }


        /// <summary>
        /// Gets or sets the function to retrieve the power factor value.
        /// </summary>
        public required Func<double?> PowerFactor { get; set; }
    }
}
