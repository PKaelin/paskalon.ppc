using System.Diagnostics.Metrics;

namespace paskalON.Telemetry.Entries
{
    /// <summary>
    /// Interface for metric entries.
    /// </summary>
    /// <typeparam name="T">The metric type.</typeparam>
    public interface IMetricEntry<T> where T : notnull
    {
        /// <summary>
        /// Name of the metric.
        /// </summary>
        string Name { get; init; }


        /// <summary>
        /// Interval of the metric.
        /// </summary>
        int Interval { get; init; }


        /// <summary>
        /// Metric type.
        /// </summary>
        MetricType MetricType { get; init; }


        /// <summary>
        /// Metric instrument.
        /// </summary>
        Instrument Instrument { get; init; }


        /// <summary>
        /// Gets the value of the instance and updates the metric value.
        /// </summary>
        /// <param name="instance">Instance from which we get the value from.</param>
        void Update(T instance);
    }
}
