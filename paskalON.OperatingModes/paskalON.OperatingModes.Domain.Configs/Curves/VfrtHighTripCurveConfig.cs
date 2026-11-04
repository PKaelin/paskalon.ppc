// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.Curves
{
    /// <summary>
    /// VfrtHighTripCurve commands the plant to completely disconnect from the grid. 
    /// This is a hard safety shutdown that requires a manual or timed reset (like 5 minutes) before the plant can make power again.
    /// </summary>
    public class VfrtHighTripCurveConfig : VfrtCurveBaseConfig
    {
        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>String representation of this instance.</returns>
        public override string ToString()
        {
            return $"{nameof(VfrtHighTripCurveConfig)} Name: {Name} XUnit: {XUnit} YUnit: {YUnit} Points: {Points.Select(p => p.ToString())}";
        }
    }
}
