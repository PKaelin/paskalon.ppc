// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.OperatingModes.Domain.Configs.Ramps
{
    /// <summary>
    /// Configuration for base for ramp ramp models.
    /// </summary>
    public class RampBaseConfig : DomainBase
    {
        /// <summary>
        /// Ramp time in seconds between enabling the operating mode and start of ramp according to model.
        /// </summary>
        public int RampTimeSeconds
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        }
    }
}
