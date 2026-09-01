// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.ClosedModes
{
    /// <summary>
    /// Base class for all closed operating mode configurations.
    /// </summary>
    public class OperatingClosedModeBaseConfig : OperatingModeBaseConfig
    {
        /// <summary>
        /// Deadband in kilo threshold used to filter minor error noise signals.
        /// </summary>
        public double DeadbandErrorKilo
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        } = 100;
    }
}
