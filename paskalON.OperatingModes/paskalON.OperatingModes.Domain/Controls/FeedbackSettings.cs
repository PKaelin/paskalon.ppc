// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Controls
{
    /// <summary>
    /// Settings of a feedback control.
    /// </summary>
    /// <param name="ProportionalGain">
    /// Proportional gain converting the error into power units (e.g. W per W, Var per V).
    /// Controller Output = Kp × Error. Only used by <see cref="FeedbackCorrection.Proportional"/>.
    /// </param>
    /// <param name="DeadbandError">Errors smaller than this value in the unit of the controlled quantity are not corrected.</param>
    public sealed record FeedbackSettings(double ProportionalGain, double DeadbandError)
    {
        /// <summary>
        /// Gets how the error adjustment is calculated.
        /// </summary>
        public FeedbackCorrection Correction { get; init; } = FeedbackCorrection.Proportional;
    }
}
