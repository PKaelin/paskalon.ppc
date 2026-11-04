// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Configs.Curves;

namespace paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageActives
{
    /// <summary>
    /// Voltage watt droop mode configuration.
    /// </summary
    public class VoltageWattDroopModeConfig : OperatingClosedModeBaseConfig
    {
        /// <summary>
        /// Curve configuration of this operating mode.
        /// </summary>
        /// <remarks>
        /// This is a droop configuration and therefore should be configured to require a curve configuration.
        /// </remarks>
        public override required CurveBaseConfig? CurveConfig
        {
            get { return field; }
            set
            {
                if (value is not FrequencyWattCurveConfig)
                {
                    throw new ArgumentException($"{nameof(CurveConfig)} must be of type {nameof(VoltWattCurveConfig)}");
                }

                field = value ?? throw new ArgumentNullException(nameof(CurveConfig));
            }
        }
    }
}
