// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Maths.Calculuses.Curves
{
    /// <summary>
    /// Defines the type of piecewise function to use for interpolation between points.
    /// </summary>
    public enum PiecewiseFunctionType
    {
        /// <summary>
        /// Linear interpolation between points. The output is calculated as a straight line between the two points.
        /// </summary>
        LinearPointFunction = 0,
        /// <summary>
        /// Exponential interpolation between points. The output is calculated using an exponential curve between the two points.
        /// </summary>
        Exponential2PointFunction = 1
    }
}
