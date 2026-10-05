// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Maths.Randoms;

namespace paskalON.Maths.UnitTest.Randoms
{
    [TestClass]
    public class RandomExtensionTest
    {
        [TestMethod]
        public void InfiniteTest()
        {
            Assert.AreEqual(double.MinValue, Random.Shared.NextDoubleInRange(double.NegativeInfinity, 0));
            Assert.AreEqual(double.MinValue, Random.Shared.NextDoubleInRange(0, double.NegativeInfinity));

            Assert.AreEqual(double.MaxValue, Random.Shared.NextDoubleInRange(double.PositiveInfinity, 0));
            Assert.AreEqual(double.MaxValue, Random.Shared.NextDoubleInRange(0, double.PositiveInfinity));
        }


        [TestMethod]
        public void MinBiggerThanMaxTest()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Random.Shared.NextDoubleInRange(10, 0));
        }

        [TestMethod]
        public void Min0Max0Test()
        {
            Assert.AreEqual(0, Random.Shared.NextDoubleInRange(0, 0));
        }


        [TestMethod]
        public void EqualNonZeroBoundsTest()
        {
            Assert.AreEqual(12.5, Random.Shared.NextDoubleInRange(12.5, 12.5));
        }


        [TestMethod]
        public void RandomDoublesInRangePositiveOnlyTest()
        {
            double ran = Random.Shared.NextDoubleInRange(0, 10);
            Assert.IsTrue(ran >= 0 && ran <= 10);

            ran = Random.Shared.NextDoubleInRange(0, double.MaxValue);
            Assert.IsTrue(ran >= 0 && ran <= double.MaxValue);

            ran = Random.Shared.NextDoubleInRange(0, 12345.12345);
            Assert.IsTrue(ran >= 0 && ran <= 12345.12345);

            ran = Random.Shared.NextDoubleInRange(0, 12.75);
            Assert.IsTrue(ran >= 0 && ran <= 12.75);

            ran = Random.Shared.NextDoubleInRange(0, 1);
            Assert.IsTrue(ran >= 0 && ran <= 1);
        }


        [TestMethod]
        public void RandomDoublesInRangeNegativeOnlyTest()
        {
            double ran = Random.Shared.NextDoubleInRange(-10, 0);
            Assert.IsTrue(ran <= 0 && ran >= -10);

            ran = Random.Shared.NextDoubleInRange(double.MinValue, 0);
            Assert.IsTrue(ran <= 0 && ran >= double.MinValue);

            ran = Random.Shared.NextDoubleInRange(-12345.12345, 0);
            Assert.IsTrue(ran <= 0 && ran >= -12345.12345);

            ran = Random.Shared.NextDoubleInRange(-12.75, 0);
            Assert.IsTrue(ran <= 0 && ran >= -12.75);

            ran = Random.Shared.NextDoubleInRange(-1, 0);
            Assert.IsTrue(ran <= 0 && ran >= -1);
        }


        [TestMethod]
        public void RandomDoublesInRangePositiveAndNegativeTest()
        {
            double ran;

            for (int i = 0; i < 100; i++)
            {
                ran = Random.Shared.NextDoubleInRange(-5, 10);
                Assert.IsTrue(ran >= -5 && ran <= 10);
            }

            for (int i = 0; i < 100; i++)
            {
                ran = Random.Shared.NextDoubleInRange(-54321.54321, 12345.12345);
                Assert.IsTrue(ran >= -54321.54321 && ran <= 12345.12345);
            }

            for (int i = 0; i < 100; i++)
            {
                ran = Random.Shared.NextDoubleInRange(-2.22, 12.75);
                Assert.IsTrue(ran >= -2.22 && ran <= 12.75);
            }

            for (int i = 0; i < 100; i++)
            {
                ran = Random.Shared.NextDoubleInRange(-1, 1);
                Assert.IsTrue(ran >= -1 && ran <= 1);
            }
        }

    }
}
