// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.UnitTest
{
    /// <summary>
    /// Tests of the class <see cref="PowerRange"/>.
    /// </summary>
    [TestClass]
    public class PowerRangeTest
    {
        /// <summary>
        /// The symmetric range goes from -|available| to +|available|, also for a negative available power.
        /// </summary>
        /// <param name="available">The available active power in watts.</param>
        [TestMethod]
        [DataRow(8000000)]
        [DataRow(-8000000)]
        public void PowerRangeSymmetricTest(double available)
        {
            PowerRange<ActivePower> range = PowerRange.Symmetric(new ActivePower(available));

            Assert.AreEqual(-8000000, range.Minimum.Watts);
            Assert.AreEqual(8000000, range.Maximum.Watts);
        }
    }
}
