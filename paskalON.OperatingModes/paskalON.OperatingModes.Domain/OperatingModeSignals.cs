// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Represents the signals of an operating mode, including base, additive, and limiting contributions for active and reactive power.
    /// </summary>
    /// <remarks>
    /// The operating mode signals are passed into every operating mode and are used to calculate the final active and reactive power outputs.
    /// The base signals are passed out of the base mode(s), additive signals are added by the additive mode(s), and limiting signals are limited by the limiting mode(s).
    /// </remarks>
    public class OperatingModeSignals
    {
        /// <summary>
        /// Gets the active power setpoint received from the supervisory control.
        /// </summary>
        /// <remarks>
        /// Read by the exclusive and base active mode.
        /// </remarks>
        public required ActivePower SetpointActivePower { get; init; }


        /// <summary>
        /// Gets the reactive power setpoint received from the supervisory control.
        /// </summary>
        /// <remarks>
        /// Read by the exclusive and base reactive mode.
        /// </remarks>
        public required ReactivePower SetpointReactivePower { get; init; }


        /// <summary>
        /// Gets or sets the base active power of the operating mode.
        /// </summary>
        /// <remarks>
        /// Passed out of the base mode(s)
        /// </remarks>
        public ActivePower BaseActivePower { get; set; } = new ActivePower(0);


        /// <summary>
        /// Gets or sets the base reactive power of the operating mode.
        /// </summary>
        /// <remarks>
        /// Passed out of the base mode(s)
        /// </remarks>
        public ReactivePower BaseReactivePower { get; set; } = new ReactivePower(0);


        /// <summary>
        /// Gets or sets the additive active power delta of the operating mode.
        /// </summary>
        /// <remarks>
        /// Added by the additive mode(s).
        /// </remarks>
        public ActivePower AdditiveActivePowerDelta { get; set; } = new ActivePower(0);


        /// <summary>
        /// Gets or sets the additive reactive power delta of the operating mode.
        /// </summary>
        /// <remarks>
        /// Added by the additive mode(s).
        /// </remarks>
        public ReactivePower AdditiveReactivePowerDelta { get; set; } = new ReactivePower(0);


        /// <summary>
        /// Gets or sets the limited active power of the operating mode.
        /// </summary>
        /// <remarks>
        /// Limited by the limiting mode(s).
        /// </remarks>
        public ActivePower? LimitedActivePower { get; set; }


        /// <summary>
        /// Gets or sets the limited reactive power of the operating mode.
        /// </summary>
        /// <remarks>
        /// Limited by the limiting mode(s).
        /// </remarks>
        public ReactivePower? LimitedReactivePower { get; set; }


        /// <summary>
        /// Gets the final active power of the operating mode, considering base, additive, and limiting contributions.
        /// </summary>
        public ActivePower FinalActivePower
        {
            get
            {
                if (LimitedActivePower.HasValue)
                {
                    return LimitedActivePower.Value;
                }

                return BaseActivePower + AdditiveActivePowerDelta;
            }
        }


        /// <summary>
        /// Gets the final reactive power of the operating mode, considering base, additive, and limiting contributions.
        /// </summary>
        public ReactivePower FinalReactivePower
        {
            get
            {
                if (LimitedReactivePower.HasValue)
                {
                    return LimitedReactivePower.Value;
                }

                return BaseReactivePower + AdditiveReactivePowerDelta;
            }
        }
    }
}
