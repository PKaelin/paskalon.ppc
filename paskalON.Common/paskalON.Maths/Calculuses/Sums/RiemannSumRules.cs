// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Maths.Calculuses.Sums
{
    /// <summary>
    /// The rule which should be used to average a Time Series. 
    /// Left Riemann Sums, Right Riemann Sums, or Trapezoidal Sums.
    /// </summary>
    public enum RiemannSumRules
    {
        RiemannLeft,
        RiemannRight,
        Trapezoidal
    }
}
