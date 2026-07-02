using paskalON.Domains.Contracts;
using paskalON.Domains.Telemetry;

namespace paskalON.Devices.Domain
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Base interface for all DER devices providing access to metrics publishing and data setting functionalities.
    /// </summary>
    /// <typeparam name="T">The type of the DER device.</typeparam>
    public interface IDevice<T>
    {
        /// <summary>
        /// Metrics getters for publishing metrics related to the DER device.
        /// </summary>
        IMetricsPublisher<T> MetricsPublisher { get; }


        /// <summary>
        /// Data setters for total loose coupled interfaces.
        /// </summary>
        IDataface<T> Dataface { get; }
    }
}
