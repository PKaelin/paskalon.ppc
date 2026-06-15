namespace paskalON.Maths.Calculuses.Sums
{
    /// <summary>
    /// Class for computing Riemann Sums, which are a method of approximating the integral of a function over an interval 
    /// by dividing the area under the curve into rectangles and summing their areas.
    /// </summary>
    public class RiemannSum
    {
        /// <summary>
        /// Averages a Time Series in raw form
        /// Computes the average value over the provided interval. If data is not provided to cover
        /// the entire interval, some of the data points will be extrapolated to fill in the blanks
        /// Multiple diagrams are shown throughout to explain what range of time the Riemann value is
        /// being computed for. To explain these diagrams you will find a "Legend" below.
        /// </summary>
        /// <param name="averagingRule">The <see cref="RiemannSumRules"/> which should be used.</param>
        /// <param name="rawValues">The raw Time Series values to be averaged.</param>
        /// <param name="intervalStart">The start of the interval to average over.</param>
        /// <param name="intervalEnd">The end of the interval to average over.</param>
        /// <returns><see cref="null"/> if the time series is empty, otherwise, the average value.</returns>
        /// <remarks>
        /// integral({a}{b}) f(x) dx =~ sum({i=1}{n−1} * (x[i+1] − x[i]) f(x[i]​)
        /// </remarks>
        public double? AverageTimeSeries(RiemannSumRules averagingRule, IEnumerable<KeyValuePair<DateTime, double>> rawValues,
            DateTime intervalStart, DateTime intervalEnd)
        {
            // Compute the total length of this interval.
            TimeSpan intervalLength = intervalEnd - intervalStart;
            int count = rawValues.Count();

            // No valid data, return null.
            if (count == 0)
            {
                return null;
            }

            // If there are values, order by time ascending. Convert to a list for array accessing.
            List<KeyValuePair<DateTime, double>> values = rawValues.OrderBy(x => x.Key).ToList();

            // Only one value, average of one value is... that value.
            if (count == 1)
            {
                return values.First().Value;
            }
            // Compute the left and right riemann sums.
            else
            {
                double riemannLeft = 0;
                double riemannRight = 0;

                for (int i = 0; i < count; i++)
                {
                    // - intervalStart: start of interval (obvious)
                    // - intervalEnd: end of interval (obvious)
                    // - currentTime: values[i].key (the timestamp of the current value)
                    // - "|---|" indicates the timespan over which the value is calculated. (the width of the rectangle)
                    // - the bottom line represents time
                    //     - "x" indicates a datapoint on that timeline
                    //     - "." indicates an interval bound (with no assigned data point)
                    if (i == 0) // This is the first data point in the time series.
                    {
                        //      intervalStart    intervalEnd
                        //      |                          |
                        //      |    currentTime           |
                        //      V    V                     V
                        //      |----------|
                        // TIME .____x_____x_______x_______.
                        riemannLeft += ComputeRiemannRectangle(values[i].Value, intervalStart, values[i + 1].Key, intervalLength);

                        //      intervalStart    intervalEnd
                        //      |                          |
                        //      |    currentTime           |
                        //      V    V                     V
                        //      |----|
                        // TIME .____x_____x_______x_______.
                        riemannRight += ComputeRiemannRectangle(values[i].Value, intervalStart, values[i].Key, intervalLength);
                    }
                    else if (i == count - 1) // This is the last data point in the time series.
                    {
                        //      intervalStart    intervalEnd
                        //      |                          |
                        //      |        currentTime       |
                        //      V                  V       V
                        //                         |-------|
                        // TIME .____x_____x_______x_______.
                        riemannLeft += ComputeRiemannRectangle(values[i].Value, values[i].Key, intervalEnd, intervalLength);

                        //      intervalStart    intervalEnd
                        //      |                          |
                        //      |        currentTime       |
                        //      V                  V       V
                        //                 |---------------|
                        // TIME .____x_____x_______x_______.
                        riemannRight += ComputeRiemannRectangle(values[i].Value, values[i - 1].Key, intervalEnd, intervalLength);
                    }
                    else // This is a data point which is bounded by other data points on each side.
                    {
                        //      intervalStart    intervalEnd
                        //      |                          |
                        //      |        currentTime       |
                        //      V          V               V
                        //                 |-------|
                        // TIME .____x_____x_______x_______.
                        riemannLeft += ComputeRiemannRectangle(values[i].Value, values[i].Key, values[i + 1].Key, intervalLength);

                        //      intervalStart    intervalEnd
                        //      |                          |
                        //      |        currentTime       |
                        //      V          V               V
                        //           |-----|
                        // TIME .____x_____x_______x_______.
                        riemannRight += ComputeRiemannRectangle(values[i].Value, values[i - 1].Key, values[i].Key, intervalLength);
                    }
                }

                // Return the selected summation type.
                switch (averagingRule)
                {
                    case RiemannSumRules.RiemannLeft:
                        return riemannLeft;
                    case RiemannSumRules.RiemannRight:
                        return riemannRight;
                    case RiemannSumRules.Trapezoidal:
                        return (riemannLeft + riemannRight) / 2;
                    default:
                        throw new InvalidOperationException("Invalid AveragingMethod selection.");
                }
            }
        }


        /// <summary>
        /// Calculates an individual rectangular value which when summed with others creates a Riemann Sum.
        /// </summary>
        /// <param name="value">The height of the rectangle (value of the datapoint)</param>
        /// <param name="subIntervalStart">The left bound of the triangle as a DateTime</param>
        /// <param name="subIntervalEnd">The right bound of the triangle as a DateTime</param>
        /// <param name="totalIntervalLength">
        /// The total timespan of the Time Series being averaged.
        /// </param>
        /// <returns>
        /// An aggregated value over a period in time (for a given interval).
        /// </returns>
        private double ComputeRiemannRectangle(double value, DateTime subIntervalStart, DateTime subIntervalEnd, TimeSpan totalIntervalLength)
        {
            TimeSpan subInterval = subIntervalEnd - subIntervalStart;

            return subInterval == TimeSpan.Zero ? 0 : value * (subInterval / totalIntervalLength);
        }
    }
}
