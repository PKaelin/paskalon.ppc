// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Configs.Curves;

namespace paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageActives
{
    /// <summary>
    /// Voltage watt droop mode configuration.
    /// </summary
    public class VoltageWattDroopModeConfig : OperatingModeBaseConfig
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
