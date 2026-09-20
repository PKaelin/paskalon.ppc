// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Percentages;

namespace paskalON.PhysicalUnits.UnitTest.Percentages
{
    [TestClass]
    public sealed class StateOfChargeTest
    {
        [TestMethod]
        [DataRow(100, 20, 80, 100, 80)]
        [DataRow(0, 20, 80, 100, 20)]
        [DataRow(50, 20, 80, 100, 50)]
        [DataRow(-16.67, 20, 80, 100, 10)]
        [DataRow(116.67, 20, 80, 100, 90)]
        [DataRow(-33.33, 20, 80, 100, 0)]
        [DataRow(133.33, 20, 80, 100, 100)]
        [DataRow(-50, 20, 80, 100, -10)]
        [DataRow(150, 20, 80, 100, 110)]
        public void GetAbsoluteStateOfChargeFromPreferredTest(double preferredSoc, double preferredMinimumSoc, double preferredMaximumSoc,
            double absoluteMaximumSoc, double expected)
        {
            double result = StateOfChargeCalculator.GetAbsoluteStateOfChargeFromPreferred(preferredSoc, preferredMinimumSoc,
                preferredMaximumSoc, absoluteMaximumSoc);

            Assert.AreEqual(expected, Math.Round(result, 2));
        }


        [TestMethod]
        [DataRow(null, 10, 90, 100, null)]
        [DataRow(0.0, 10, 90, 100, 10.0)]
        [DataRow(10.0, 10, 90, 100, 18.0)]
        [DataRow(20.0, 10, 90, 100, 26.0)]
        [DataRow(25.0, 10, 90, 100, 30.0)]
        [DataRow(50.0, 10, 90, 100, 50.0)]
        [DataRow(75.0, 10, 90, 100, 70.0)]
        [DataRow(80.0, 10, 90, 100, 74.0)]
        [DataRow(90.0, 10, 90, 100, 82.0)]
        [DataRow(100.0, 10, 90, 100, 90.0)]
        public void GetAbsoluteStateOfChargeFromUsableTest(double? usableSoc, double usableMinimumSoc, double usableMaximumSoc,
            double absoluteMaximumSoc, double? expected)
        {
            double? result = StateOfChargeCalculator.GetAbsoluteStateOfChargeFromUsable(usableSoc, usableMinimumSoc,
                usableMaximumSoc, absoluteMaximumSoc);

            Assert.AreEqual(expected, result == null ? (double?)null : Math.Round((double)result, 2));
        }


        [TestMethod]
        [DataRow(null, 20, 80, 100, null)]
        [DataRow(0.0, 20, 80, 100, -33.33)]
        [DataRow(10.0, 20, 80, 100, -16.67)]
        [DataRow(20.0, 20, 80, 100, 0.00)]
        [DataRow(30.0, 20, 80, 100, 16.67)]
        [DataRow(40.0, 20, 80, 100, 33.33)]
        [DataRow(50.0, 20, 80, 100, 50.00)]
        [DataRow(60.0, 20, 80, 100, 66.67)]
        [DataRow(70.0, 20, 80, 100, 83.33)]
        [DataRow(80.0, 20, 80, 100, 100.00)]
        [DataRow(90.0, 20, 80, 100, 116.67)]
        [DataRow(100.0, 20, 80, 100, 133.33)]
        public void GetPreferredStateOfChargeFromAbsoluteTest(double? absoluteSoc, double preferredMinimumStateOfCharge,
            double preferredMaximumStateOfCharge, double absoluteMaximumStateOfCharge, double? expected)
        {
            double? result = StateOfChargeCalculator.GetPreferredStateOfChargeFromAbsolute(absoluteSoc, preferredMinimumStateOfCharge,
                preferredMaximumStateOfCharge, absoluteMaximumStateOfCharge);

            Assert.AreEqual(expected, result == null ? (double?)null : Math.Round((double)result, 2));
        }


        [TestMethod]
        [DataRow(0, 10, 90, 100, -12.5)]
        [DataRow(10, 10, 90, 100, 0)]
        [DataRow(20, 10, 90, 100, 12.5)]
        [DataRow(30, 10, 90, 100, 25)]
        [DataRow(40, 10, 90, 100, 37.5)]
        [DataRow(50, 10, 90, 100, 50)]
        [DataRow(60, 10, 90, 100, 62.5)]
        [DataRow(70, 10, 90, 100, 75)]
        [DataRow(80, 10, 90, 100, 87.5)]
        [DataRow(90, 10, 90, 100, 100)]
        [DataRow(100, 10, 90, 100, 112.5)]
        public void GetUsableStateOfChargeFromAbsoluteTest(double absoluteSoc, double usableMinimumSoc, double usableMaximumSoc,
            double absoluteMaximumSoc, double expected)
        {
            double result = StateOfChargeCalculator.GetUsableStateOfChargeFromAbsolute(absoluteSoc, usableMinimumSoc,
                usableMaximumSoc, absoluteMaximumSoc);

            Assert.AreEqual(expected, Math.Round(result, 2));
        }


        [TestMethod]
        [DataRow(5000000, 10, 90, 0, 100, 4000000)]
        [DataRow(5000000, 20, 80, 0, 100, 3000000)]
        [DataRow(5000000, 0, 100, 0, 100, 5000000)]
        [DataRow(5000000, 10, 90, 10, 90, 5000000)]
        [DataRow(1000000, 10, 90, 0, 100, 800000)]
        [DataRow(10000000, 10, 90, 0, 100, 8000000)]
        [DataRow(5000000, 25, 75, 0, 100, 2500000)]
        [DataRow(5000000, 10, 80, 0, 100, 3500000)]
        [DataRow(5000000, 20, 90, 0, 100, 3500000)]
        public void GetUsableCapacityTest(double nameplateCapacity, double usableMinimumSoc,
            double usableMaximumSoc, double absoluteMinimumSoc, double absoluteMaximumSoc, double expected)
        {
            double result = StateOfChargeCalculator.GetUsableCapacity(nameplateCapacity, usableMinimumSoc, usableMaximumSoc,
                absoluteMinimumSoc, absoluteMaximumSoc);

            Assert.AreEqual(expected, Math.Round(result, 2));
        }
    }
}
