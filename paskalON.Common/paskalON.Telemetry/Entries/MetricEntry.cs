using System.Diagnostics.Metrics;

namespace paskalON.Telemetry.Entries
{
    /// <summary>
    /// Metric entry to store registered metric points.
    /// </summary>
    /// <typeparam name="T">The metric type.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public class MetricEntry<T, TProperty> : IMetricEntry<T> where T : notnull where TProperty : struct
    {
        /// <summary>
        /// Function with which we can get the value to then update the metric with.
        /// </summary>
        private readonly Func<T, TProperty?> _getter;

        /// <summary>
        /// Action with which we can update the metric (instrument) value.
        /// </summary>
        private readonly Action<TProperty> _updater;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public required int Interval { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public MetricType MetricType { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public required Instrument Instrument { get; init; }


        /// <summary>
        /// Constructor of <see cref="IMetricEntry{T}"/>.
        /// </summary>
        /// <param name="instrument">The metric instrument.</param>
        /// <param name="getter">The getter function to get the value with.</param>
        /// <exception cref="NotImplementedException">Throw an exception when the property type is not implemented.</exception>
        public MetricEntry(Instrument instrument, Func<T, TProperty?> getter)
        {
            _getter = getter;

            if (instrument is Counter<TProperty> counter)
            {
                _updater = value => counter.Add(value);
            }
            else if (instrument is UpDownCounter<TProperty> up_down)
            {
                _updater = value => up_down.Add(value);
            }
            else if (instrument is Gauge<TProperty> gauge)
            {
                _updater = value => gauge.Record(value);
            }
            else if (instrument is Histogram<TProperty> histogram)
            {
                _updater = value => histogram.Record(value);
            }
            else
            {
                throw new NotImplementedException($"Instrument type: {instrument.GetType().Name} is not implemented.");
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Update(T instance)
        {
            TProperty? property = _getter(instance);
            // Do not update nullable values.
            if (property != null)
            {
                _updater((TProperty)property);
            }
        }

    }
}
