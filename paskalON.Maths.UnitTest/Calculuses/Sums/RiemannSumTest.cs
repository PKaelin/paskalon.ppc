using paskalON.Maths.Calculuses.Sums;
using System.Collections.Immutable;

namespace paskalON.Maths.UnitTest.Calculuses.Sums
{
    [TestClass]
    public class RiemannSumTest
    {
        private RiemannSum riemannSum = new RiemannSum();

        private const double UniformValueWeight = 3.0 / 15.0;

        // The bounds of the interval which will be used in these tests.
        private static readonly DateTime IntervalStart = DateTime.Parse("2017-04-01T04:00:00Z");
        private static readonly DateTime IntervalEnd = DateTime.Parse("2017-04-01T04:15:00Z");
        private static readonly double IntervalMinutes = (IntervalEnd - IntervalStart).TotalMinutes;


        // Create a time series with made up values. The granularity between the points will be uniform.
        private static readonly ImmutableList<KeyValuePair<DateTime, double>> UniformTimeSeries = ImmutableList.Create<KeyValuePair<DateTime, double>>
        (
            new KeyValuePair<DateTime, double>(DateTime.Parse("2017-04-01T04:00:00Z"), 2),
            new KeyValuePair<DateTime, double>(DateTime.Parse("2017-04-01T04:03:00Z"), 8),
            new KeyValuePair<DateTime, double>(DateTime.Parse("2017-04-01T04:06:00Z"), 3),
            new KeyValuePair<DateTime, double>(DateTime.Parse("2017-04-01T04:09:00Z"), 1),
            new KeyValuePair<DateTime, double>(DateTime.Parse("2017-04-01T04:12:00Z"), 4),
            new KeyValuePair<DateTime, double>(DateTime.Parse("2017-04-01T04:15:00Z"), 6)
        );


        // Create a time series with made up values. The granularity between the points will be non-uniform.
        private static readonly ImmutableList<KeyValuePair<DateTime, double>> NonUniformTimeSeries = ImmutableList.Create<KeyValuePair<DateTime, double>>
        (
            new KeyValuePair<DateTime, double>(DateTime.Parse("2017-04-01T04:01:00Z"), 2),
            new KeyValuePair<DateTime, double>(DateTime.Parse("2017-04-01T04:05:00Z"), 8),
            new KeyValuePair<DateTime, double>(DateTime.Parse("2017-04-01T04:06:00Z"), 3),
            new KeyValuePair<DateTime, double>(DateTime.Parse("2017-04-01T04:08:00Z"), 1),
            new KeyValuePair<DateTime, double>(DateTime.Parse("2017-04-01T04:13:00Z"), 4),
            new KeyValuePair<DateTime, double>(DateTime.Parse("2017-04-01T04:14:00Z"), 6)
        );


        // Hand compute the Left and Right Riemann Sums for both Time Series's
        private static readonly double ExpectedUniformLeftRiemannSum =
            (UniformTimeSeries[0].Value * UniformValueWeight) +
            (UniformTimeSeries[1].Value * UniformValueWeight) +
            (UniformTimeSeries[2].Value * UniformValueWeight) +
            (UniformTimeSeries[3].Value * UniformValueWeight) +
            (UniformTimeSeries[4].Value * UniformValueWeight);

        private static readonly double ExpectedUniformRightRiemannSum =
            (UniformTimeSeries[1].Value * UniformValueWeight) +
            (UniformTimeSeries[2].Value * UniformValueWeight) +
            (UniformTimeSeries[3].Value * UniformValueWeight) +
            (UniformTimeSeries[4].Value * UniformValueWeight) +
            (UniformTimeSeries[5].Value * UniformValueWeight);

        private static readonly double ExpectedNonUniformLeftRiemannSum =
            (UniformTimeSeries[0].Value * (NonUniformTimeSeries[1].Key - IntervalStart).TotalMinutes / IntervalMinutes) +
            (UniformTimeSeries[1].Value * (NonUniformTimeSeries[2].Key - NonUniformTimeSeries[1].Key).TotalMinutes / IntervalMinutes) +
            (UniformTimeSeries[2].Value * (NonUniformTimeSeries[3].Key - NonUniformTimeSeries[2].Key).TotalMinutes / IntervalMinutes) +
            (UniformTimeSeries[3].Value * (NonUniformTimeSeries[4].Key - NonUniformTimeSeries[3].Key).TotalMinutes / IntervalMinutes) +
            (UniformTimeSeries[4].Value * (NonUniformTimeSeries[5].Key - NonUniformTimeSeries[4].Key).TotalMinutes / IntervalMinutes) +
            (UniformTimeSeries[5].Value * (IntervalEnd - NonUniformTimeSeries[5].Key).TotalMinutes / IntervalMinutes);

        private static readonly double ExpectedNonUniformRightRiemannSum =
            (UniformTimeSeries[0].Value * (NonUniformTimeSeries[0].Key - IntervalStart).TotalMinutes / IntervalMinutes) +
            (UniformTimeSeries[1].Value * (NonUniformTimeSeries[1].Key - NonUniformTimeSeries[0].Key).TotalMinutes / IntervalMinutes) +
            (UniformTimeSeries[2].Value * (NonUniformTimeSeries[2].Key - NonUniformTimeSeries[1].Key).TotalMinutes / IntervalMinutes) +
            (UniformTimeSeries[3].Value * (NonUniformTimeSeries[3].Key - NonUniformTimeSeries[2].Key).TotalMinutes / IntervalMinutes) +
            (UniformTimeSeries[4].Value * (NonUniformTimeSeries[4].Key - NonUniformTimeSeries[3].Key).TotalMinutes / IntervalMinutes) +
            (UniformTimeSeries[5].Value * (IntervalEnd - NonUniformTimeSeries[4].Key).TotalMinutes / IntervalMinutes);




        [TestMethod]
        public void RiemannSumComputesRiemannLeft()
        {
            double? actualUniformAverage = riemannSum.AverageTimeSeries(RiemannSumRules.RiemannLeft, UniformTimeSeries, IntervalStart, IntervalEnd);
            Assert.AreEqual(ExpectedUniformLeftRiemannSum, actualUniformAverage);

            double? actualNonUniformAverage = riemannSum.AverageTimeSeries(RiemannSumRules.RiemannLeft, NonUniformTimeSeries, IntervalStart, IntervalEnd);
            Assert.AreEqual(ExpectedNonUniformLeftRiemannSum, actualNonUniformAverage);
        }


        [TestMethod]
        public void RiemannSumComputesRiemannRight()
        {
            double? actualUniformAverage = riemannSum.AverageTimeSeries(RiemannSumRules.RiemannRight, UniformTimeSeries, IntervalStart, IntervalEnd);
            Assert.AreEqual(ExpectedUniformRightRiemannSum, actualUniformAverage);

            double? actualNonUniformAverage = riemannSum.AverageTimeSeries(RiemannSumRules.RiemannRight, NonUniformTimeSeries, IntervalStart, IntervalEnd);
            Assert.AreEqual(ExpectedNonUniformRightRiemannSum, actualNonUniformAverage);
        }


        [TestMethod]
        public void RiemannSumComputesTrapezoidalSum()
        {
            double? actualUniformAverage = riemannSum.AverageTimeSeries(RiemannSumRules.Trapezoidal, UniformTimeSeries, IntervalStart, IntervalEnd);
            Assert.AreEqual((ExpectedUniformLeftRiemannSum + ExpectedUniformRightRiemannSum) / 2, actualUniformAverage);

            double? actualNonUniformAverage = riemannSum.AverageTimeSeries(RiemannSumRules.Trapezoidal, NonUniformTimeSeries, IntervalStart, IntervalEnd);
            Assert.AreEqual((ExpectedNonUniformLeftRiemannSum + ExpectedNonUniformRightRiemannSum) / 2, actualNonUniformAverage);
        }


        [TestMethod]
        public void RiemannSumHandlesOneValue()
        {
            double expectedValue = 5;
            List<KeyValuePair<DateTime, double>> singleValueTimeSeries = new List<KeyValuePair<DateTime, double>>()
            {
                new KeyValuePair<DateTime, double>(IntervalStart, expectedValue)
            };

            double? averageLeft = riemannSum.AverageTimeSeries(RiemannSumRules.RiemannLeft, singleValueTimeSeries, IntervalStart, IntervalEnd);
            double? averageRight = riemannSum.AverageTimeSeries(RiemannSumRules.RiemannRight, singleValueTimeSeries, IntervalStart, IntervalEnd);
            double? averageTrapezoidal = riemannSum.AverageTimeSeries(RiemannSumRules.Trapezoidal, singleValueTimeSeries, IntervalStart, IntervalEnd);

            Assert.AreEqual(expectedValue, averageLeft);
            Assert.AreEqual(expectedValue, averageRight);
            Assert.AreEqual(expectedValue, averageTrapezoidal);
        }


        [TestMethod]
        public void RiemannSumHandlesZeroValue()
        {
            double? expectedValue = null;
            List<KeyValuePair<DateTime, double>> singleValueTimeSeries = new List<KeyValuePair<DateTime, double>>();

            double? averageLeft = riemannSum.AverageTimeSeries(RiemannSumRules.RiemannLeft, singleValueTimeSeries, IntervalStart, IntervalEnd);
            double? averageRight = riemannSum.AverageTimeSeries(RiemannSumRules.RiemannRight, singleValueTimeSeries, IntervalStart, IntervalEnd);
            double? averageTrapezoidal = riemannSum.AverageTimeSeries(RiemannSumRules.Trapezoidal, singleValueTimeSeries, IntervalStart, IntervalEnd);

            Assert.AreEqual(expectedValue, averageLeft);
            Assert.AreEqual(expectedValue, averageRight);
            Assert.AreEqual(expectedValue, averageTrapezoidal);
        }
    }
}
