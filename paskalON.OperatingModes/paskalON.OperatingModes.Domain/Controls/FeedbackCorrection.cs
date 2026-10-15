// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Controls
{
    /// <summary>
    /// Defines how a feedback control calculates its error adjustment.
    /// </summary>
    public enum FeedbackCorrection
    {
        /// <summary>
        /// Error adjustment = proportional gain × error. Leaves a residual error in steady state.
        /// </summary>
        Proportional,


        /// <summary>
        /// The error adjustment accumulates the full error and moves there with the ramp of the operating mode.
        /// </summary>
        /// <remarks>
        /// Removes the steady-state error completely without a gain. A new error is only accumulated once the previous correction
        /// is reached and the measurement changed. Requires the controlled quantity in the power unit of the axis.
        /// </remarks>
        RampLimitedIntegral,
    }
}
