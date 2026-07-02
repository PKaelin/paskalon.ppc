using paskalON.Domains.Contracts;

namespace paskalON.Domains.Telemetry
{
    /// <summary>
    /// Interface for registering and publishing metrics for a given type T.
    /// </summary>
    /// <typeparam name="T">The type of metrics to register and publish.</typeparam>
    public interface IMetricsPublisher<T> : IPropertyGetter<T>
    {
        /// <summary>
        /// Publishes the metrics for the given instance at the specified interval.
        /// </summary>
        /// <param name="instance">The instance of T for which to publish metrics.</param>
        /// <param name="interval">The interval at which to publish the metrics.</param>
        void Publish(T instance, int interval);
    }
}
