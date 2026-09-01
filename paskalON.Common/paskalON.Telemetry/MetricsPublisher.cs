// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Telemetry.Entries;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace paskalON.Telemetry
{
    /// <summary>
    /// Metrics publisher publishes metrics using the .NET System.Diagnostics.Metrics namespace.
    /// </summary>
    /// <typeparam name="T">Type of the instance.</typeparam>
    public class MetricsPublisher : IMetricsPublisher
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public bool IsEnabled { get; set; } = true;


        /// <summary>
        /// Caches the metrics structure.
        /// </summary>
        private readonly Dictionary<string, IMetricEntry> _metrics = new Dictionary<string, IMetricEntry>();


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
                throw new ApplicationException($"Metrics publisher has already been initialized. Measurement: {measurement}");
            }

            Meter = new Meter(measurement);
            _tags = tags;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Register<TDevice, TProperty>(TDevice instance, string name, MetricType metricType, Func<TDevice, TProperty?> getter, int interval = 1) where TProperty : struct
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(getter);

            if (Meter == null)
            {
                throw new ApplicationException($"Metrics publisher must be initialized first. Type: {typeof(TDevice).Name}");
            }

            if (_metrics.ContainsKey(name) == true)
            {
                throw new ArgumentException($"Name has to be unique when registering metrics publisher. Type: {typeof(TDevice).Name} Name: {name}");
            }

            Instrument<TProperty> instrument;

            if (metricType == MetricType.Counter)
            {
                instrument = Meter.CreateCounter<TProperty>($"{Meter.Name.ToLower()}_{name.ToLower()}");
            }
            else if (metricType == MetricType.UpDownCounter)
            {
                instrument = Meter.CreateUpDownCounter<TProperty>($"{Meter.Name.ToLower()}_{name.ToLower()}");
            }
            else if (metricType == MetricType.Gauge)
            {
                instrument = Meter.CreateGauge<TProperty>($"{Meter.Name.ToLower()}_{name.ToLower()}");
            }
            else if (metricType == MetricType.Histogram)
            {
                instrument = Meter.CreateHistogram<TProperty>($"{Meter.Name.ToLower()}_{name.ToLower()}");
            }
            else
            {
                throw new NotImplementedException($"Instrument type: {metricType} is not implemented.");
            }

            TagList tagList = new TagList(_tags.ToArray());
            _metrics.Add(name, new MetricEntry<TDevice, TProperty>(instance, name, instrument, metricType, getter, tagList, interval));
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Publish(int currentInterval)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(currentInterval);

            if (IsEnabled == true)
            {
                if (Meter == null)
                {
                    throw new ApplicationException($"Metrics publisher must be initialized first.");
                }

                foreach (IMetricEntry entry in _metrics.Values)
                {
                    if (currentInterval % entry.Interval == 0)
                    {
                        entry.Update();
                    }
                }
            }
        }
    }
}

