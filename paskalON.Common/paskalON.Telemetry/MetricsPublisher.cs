// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Telemetry.Entries;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace paskalON.Telemetry
{
    /// <summary>
    /// Metrics publisher publishes metrics using the .NET System.Diagnostics.Metrics namespace.
    /// </summary>
    /// <typeparam name="T">Type of the instance.</typeparam>
    public class MetricsPublisher : IMetricsPublisher, IDisposable
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<MetricsPublisher> _logger;


        /// <inheritdoc />
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
        /// Constructor of <see cref="MetricsPublisher"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        public MetricsPublisher(ILogger<MetricsPublisher> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }


        /// <inheritdoc/>
        public void Initialize(string measurement, IEnumerable<KeyValuePair<string, object?>> tags)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(measurement);
            ArgumentNullException.ThrowIfNull(tags);

            if (Meter != null)
            {
                throw new InvalidOperationException($"Metrics publisher has already been initialized. Measurement: {measurement}");
            }

            Meter = new Meter(measurement);
            _tags = tags;
        }


        /// <inheritdoc/>
        public void Register<TDevice, TProperty>(TDevice instance, string name, MetricType metricType, Func<TDevice, TProperty?> getter, int interval = 1) where TProperty : struct
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(getter);

            if (Meter == null)
            {
                throw new ApplicationException($"Metrics publisher must be initialized first. Type: {typeof(TDevice).Name}");
            }

            if (_metrics.ContainsKey(name.ToLower()) == true)
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
            _metrics.Add(name.ToLower(), new MetricEntry<TDevice, TProperty>(instance, name, instrument, metricType, getter, tagList, interval));
        }


        /// <inheritdoc/>
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
                        try
                        {
                            entry.Update();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"Failed to update metric entry. Name: {entry.Name} Type: {entry.MetricType} Exception: {ex.Message}");
                        }
                    }
                }
            }
        }


        /// <inheritdoc/>
        public void Dispose()
        {
            if (Meter != null)
            {
                Meter.Dispose();
                Meter = null;
                _metrics.Clear();
            }
        }
    }
}
