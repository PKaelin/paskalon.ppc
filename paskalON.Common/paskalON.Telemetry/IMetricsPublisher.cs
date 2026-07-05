// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Telemetry
{
    /// <summary>
    /// Interface for registering and publishing metrics for a given type T.
    /// </summary>
    /// <typeparam name="T">The type of metrics to register and publish.</typeparam>
    public interface IMetricsPublisher<T> where T : notnull
    {
        /// <summary>
        /// Initialized the metrics publisher instance.
        /// </summary>
        /// <param name="measurement">The name of the measurement for the metrics</param>
        /// <param name="tags">Tags for the measurement.</param>
        void Initialize(string measurement, IEnumerable<KeyValuePair<string, object?>> tags);


        /// <summary>
        /// Registers a property with the specified name, getter function and optional interval.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to register.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="metricType">Metric type <see cref="MetricType"/>.</param>
        /// <param name="getter">A function to get the value of the property from an instance of T.</param>
        /// <param name="interval">The interval at which to publish the property if publishing is required.</param>
        /// /// <remarks>
        /// Syntax func: nameof(property/field), x => x.PropertyName/x.FieldName;
        /// </remarks>
        void Register<TProperty>(string name, MetricType metricType, Func<T, TProperty?> getter, int interval = 1) where TProperty : struct;


        /// <summary>
        /// Publishes the metrics for the given instance at the specified interval.
        /// </summary>
        /// <param name="instance">The instance of T for which to publish metrics.</param>
        /// <param name="interval">The interval at which to publish the metrics.</param>
        void Publish(T instance, int interval);
    }
}
