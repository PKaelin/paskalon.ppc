using paskalON.Telemetry.Entries;
using System.Diagnostics.Metrics;

namespace paskalON.Telemetry
{
    /// <summary>
    /// Metrics publisher publishes metrics using the .NET System.Diagnostics.Metrics namespace.
    /// </summary>
    /// <typeparam name="T">Type of the instance.</typeparam>
    public class MetricsPublisher<T> : IMetricsPublisher<T> where T : notnull
    {
        /// <summary>
        /// Caches the metrics structure.
        /// </summary>
        private readonly Dictionary<string, IMetricEntry<T>> _metrics = new Dictionary<string, IMetricEntry<T>>();


        /// <summary>
        /// Tags for the measurements.
        /// </summary>
        private IEnumerable<KeyValuePair<string, object?>> _tags = [];

        /// <summary>
        /// The logical factory or container that groups related instruments.
        /// </summary>
        public Meter? Meter { get; private set; }


        /// <summary>
        /// Indicates whether the metrics publisher initialized method has been called.
        /// </summary>
        public bool IsInitialized { get => Meter != null; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Initialize(string measurement, IEnumerable<KeyValuePair<string, object?>> tags)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(measurement);
            ArgumentNullException.ThrowIfNull(tags);

            if (Meter != null)
            {
                throw new ApplicationException($"Metrics publisher has already been initialized. Type: {typeof(T).Name}");
            }

            Meter = new Meter(measurement);
            _tags = tags;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Register<TProperty>(string name, MetricType metricType, Func<T, TProperty?> getter, int interval = 1) where TProperty : struct
        {
            if (_metrics.ContainsKey(name) == true)
            {
                throw new ArgumentException($"Name has to be unique when registering metrics publisher. Type: {typeof(T).Name}");
            }

            if (Meter == null)
            {
                throw new ApplicationException($"Metrics publisher must be initialized first. Type: {typeof(T).Name}");
            }

            Instrument<TProperty> instrument;

            if (metricType == MetricType.Counter)
            {
                instrument = Meter.CreateCounter<TProperty>(name, null, null, _tags);
            }
            else if (metricType == MetricType.UpDownCounter)
            {
                instrument = Meter.CreateUpDownCounter<TProperty>(name, null, null, _tags);
            }
            else if (metricType == MetricType.Gauge)
            {
                instrument = Meter.CreateGauge<TProperty>(name, null, null, _tags);
            }
            else if (metricType == MetricType.Histogram)
            {
                instrument = Meter.CreateHistogram<TProperty>(name, null, null, _tags);
            }
            else
            {
                throw new NotImplementedException($"Instrument type: {metricType} is not implemented.");
            }

            _metrics.Add(name, new MetricEntry<T, TProperty>(instrument, getter) { Name = name, MetricType = metricType, Instrument = instrument, Interval = interval });
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Publish(T instance, int interval)
        {
            if (Meter == null)
            {
                throw new ApplicationException($"Metrics publisher must be initialized first. Type: {typeof(T).Name}");
            }

            foreach (IMetricEntry<T> entry in _metrics.Values)
            {
                if (interval % entry.Interval == 0)
                {
                    entry.Update(instance);
                }
            }
        }
    }
}