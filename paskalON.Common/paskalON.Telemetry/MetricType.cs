namespace paskalON.Telemetry
{
    /// <summary>
    /// Metric type
    /// </summary>
    public enum MetricType
    {
        /// <summary>
        /// Tracks values that only increase over time.
        /// </summary>
        Counter = 0,
        /// <summary>
        /// Tracks values that can increase or decrease over time.
        /// </summary>
        UpDownCounter = 1,
        /// <summary>
        /// Tracks instantaneous or fluctuating values.
        /// </summary>
        Gauge = 2,
        /// <summary>
        /// Records multiple related values and measures their distribution.
        /// </summary>
        Histogram = 3
    }
}
