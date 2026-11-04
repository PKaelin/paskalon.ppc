// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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
