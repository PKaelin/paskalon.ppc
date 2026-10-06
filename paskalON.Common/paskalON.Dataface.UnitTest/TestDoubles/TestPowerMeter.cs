// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.UnitTest.TestDoubles
{
    /// <summary>
    /// Power meter test double used where an instance of a different device type is required.
    /// </summary>
    internal sealed class TestPowerMeter
    {
        /// <summary>
        /// Active power in watts.
        /// </summary>
        public double ActivePower { get; set; }
    }
}
