// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using System.Diagnostics.Metrics;

namespace paskalON.Telemetry.Entries
{
    /// <summary>
    /// Metric entry to store registered metric points.
    /// </summary>
    /// <typeparam name="TDevice">The metric type.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public class MetricEntry<TDevice, TProperty> : IMetricEntry where TProperty : struct
    {
        /// <summary>
        /// Function with which we can get the value to then update the metric with.
        /// Accepts an object (the instance) and returns the nullable property value.
        /// </summary>
        private readonly Func<TDevice, TProperty?> _getter;

        /// <summary>
        /// Action with which we can update the metric (instrument) value.
        /// </summary>
        private readonly Action<TProperty> _updater;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object Instance { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Name { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Interval { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public MetricType MetricType { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Instrument Instrument { get; init; }


        /// <summary>
        /// Constructor of <see cref="IMetricEntry{T}"/>.
        /// </summary>
        /// <param name="instrument">The metric instrument.</param>
        /// <param name="getter">The getter function to get the value with.</param>
        /// <exception cref="NotImplementedException">Throw an exception when the property type is not implemented.</exception>
        public MetricEntry(object instance, string name, Instrument instrument, MetricType metricType, Func<TDevice, TProperty?> getter, int interval)
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(getter);

            Instance = instance;
            Name = name;
            Instrument = instrument;
            MetricType = metricType;
            _getter = getter;
            Interval = interval;

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
        public void Update()
        {
            if (Instance is not TDevice typedDevice)
            {
                throw new ArgumentException($"{nameof(IMetricEntry)} must be of type {typeof(TDevice).Name}", nameof(Instance));
            }

            TProperty? property = _getter((TDevice)Instance);

            // Do not update nullable values.
            if (property != null)
            {
                _updater((TProperty)property);
            }
        }
    }
}
