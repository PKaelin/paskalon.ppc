// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Telemetry
{
    /// <summary>
    /// Interface for registering and publishing metrics for a given type T.
    /// </summary>
    public interface IMetricsPublisher
    {
        /// <summary>
        /// Gets or sets a value indicating whether the metrics publisher is enabled. If set to false, metrics will not be published.
        /// </summary>
        bool IsEnabled { get; set; }


        /// <summary>
        /// Initialized the metrics publisher instance.
        /// </summary>
        /// <param name="measurement">The name of the measurement for the metrics</param>
        /// <param name="tags">Tags for the measurement.</param>
        void Initialize(string measurement, IEnumerable<KeyValuePair<string, object?>> tags);


        /// <summary>
        /// Registers a property with the specified name, getter function and optional interval.
        /// </summary>
        /// <typeparam name="TDevice">The metric type.</typeparam>
        /// <typeparam name="TProperty">The type of the property to register.</typeparam>
        /// <param name="instance">Instance to use for the update.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="metricType">Metric type <see cref="MetricType"/>.</param>
        /// <param name="getter">A function to get the value of the property from an instance of T.</param>
        /// <param name="interval">The interval at which to publish the property if publishing is required.</param>
        /// /// <remarks>
        /// Syntax func: nameof(property/field), x => x.PropertyName/x.FieldName;
        /// </remarks>
        void Register<TDevice, TProperty>(TDevice instance, string name, MetricType metricType, Func<TDevice, TProperty?> getter, int interval = 1) where TProperty : struct;


        /// <summary>
        /// Publishes the metrics for the given instance at the specified interval.
        /// </summary>
        /// <param name="currentInterval">The current interval iteration.</param>
        void Publish(int currentInterval);
    }
}
