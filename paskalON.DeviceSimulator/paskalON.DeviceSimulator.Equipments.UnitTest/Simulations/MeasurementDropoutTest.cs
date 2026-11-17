// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.DeviceSimulator.Equipments.Simulations;

namespace paskalON.DeviceSimulator.Equipments.UnitTest.Simulations
{
    [TestClass]
    public sealed class MeasurementDropoutTest
    {
        [TestMethod]
        public void MeasurementDropoutIsOffByDefaultTest()
        {
            Mock<Random> random = new Mock<Random>();
            random.Setup(x => x.NextDouble()).Returns(0d);

            MeasurementDropout dropout = new MeasurementDropout(random.Object);

            Assert.IsFalse(dropout.IsEnabled);
            Assert.AreEqual(MeasurementDropout.DefaultPercent, dropout.Percent);
            Assert.IsFalse(dropout.IsDropped());
            random.Verify(x => x.NextDouble(), Times.Never);
        }


        [TestMethod]
        [DataRow(25d, 0.1d, true, DisplayName = "Random value below percent drops")]
        [DataRow(25d, 0.25d, false, DisplayName = "Random value equal to percent reports")]
        [DataRow(25d, 0.9d, false, DisplayName = "Random value above percent reports")]
        [DataRow(100d, 0.999d, true, DisplayName = "100 percent always drops")]
        [DataRow(0d, 0d, false, DisplayName = "0 percent never drops")]
        public void IsDroppedComparesRandomValueWithPercentTest(double percent, double randomValue, bool expected)
        {
            Mock<Random> random = new Mock<Random>();
            random.Setup(x => x.NextDouble()).Returns(randomValue);
            MeasurementDropout dropout = new MeasurementDropout(random.Object);
            dropout.Set(true, percent);

            bool result = dropout.IsDropped();

            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void SetDisabledReportsEveryUpdateAndKeepsPercentTest()
        {
            Mock<Random> random = new Mock<Random>();
            random.Setup(x => x.NextDouble()).Returns(0d);
            MeasurementDropout dropout = new MeasurementDropout(random.Object);
            dropout.Set(true, 50);

            dropout.Set(false, 50);

            Assert.IsFalse(dropout.IsEnabled);
            Assert.AreEqual(50, dropout.Percent);
            Assert.IsFalse(dropout.IsDropped());
        }


        [TestMethod]
        [DataRow(-0.1d)]
        [DataRow(100.1d)]
        public void SetInvalidPercentThrowsAndKeepsStateTest(double percent)
        {
            MeasurementDropout dropout = new MeasurementDropout(new Random(1));

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => dropout.Set(true, percent));
            Assert.IsFalse(dropout.IsEnabled);
            Assert.AreEqual(MeasurementDropout.DefaultPercent, dropout.Percent);
        }


        [TestMethod]
        public void MeasurementDropoutNullRandomThrowsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new MeasurementDropout(null!));
        }
    }
}
