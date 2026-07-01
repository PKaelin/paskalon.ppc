namespace paskalON.Domains.Telemetry
{
    /// <summary>
    /// Interface for registering and publishing metrics for a given type T.
    /// </summary>
    /// <typeparam name="T">The type of metrics to register and publish.</typeparam>
    public interface IMetricsPublisher<T>
    {
        /// <summary>
        /// Registers a metric with the specified name, getter function, and interval.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to register as a metric.</typeparam>
        /// <param name="name">The name of the metric.</param>
        /// <param name="getter">A function to get the value of the metric from an instance of T.</param>
        /// <param name="interval">The interval at which to publish the metric.</param>
        void Register<TProperty>(string name, Func<T, TProperty> getter, int interval);


        /// <summary>
        /// Publishes the metrics for the given instance at the specified interval.
        /// </summary>
        /// <param name="instance">The instance of T for which to publish metrics.</param>
        /// <param name="interval">The interval at which to publish the metrics.</param>
        void Publish(T instance, int interval);
    }
}
