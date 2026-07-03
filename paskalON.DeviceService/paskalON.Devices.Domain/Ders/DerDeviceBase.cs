using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs;
using paskalON.Domains.Contracts;
using paskalON.Domains.Telemetry;

namespace paskalON.Devices.Domain.Ders
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Base class for all distributed energy resources (DERs) that symbolizes a device.
    /// </summary>
    /// <typeparam name="T">The type of the DER device.</typeparam>
    public abstract class DerDeviceBase<T> : DerBase
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IMetricsPublisher<T> MetricsPublisher { get; private set; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IDataface<T> Dataface { get; private set; }


        /// <summary>
        /// Constructor of <see cref="DerDeviceBase{T}"/>
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="nameBase">The name base configuration.</param>
        /// <param name="publisher">The metrics publisher interface.</param>
        /// <param name="device">The device interface.</param>
        protected DerDeviceBase(ILogger logger, NameBase nameBase, IMetricsPublisher<T> publisher, IDevice<T> device) : base(logger, nameBase)
        {
            ArgumentNullException.ThrowIfNull(nameBase);
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(device);

            MetricsPublisher = publisher;
            Dataface = device.Dataface;
        }


        /// <summary>
        /// Register metrics at the publisher.
        /// </summary>
        /// <param name="metricsPublisher">The metrics publisher interface.</param>
        protected abstract void RegisterMetrics(IMetricsPublisher<T> metricsPublisher);


        /// <summary>
        /// Register the data interface at the property setter.
        /// </summary>
        /// <param name="dataface">The data face interface with property setter.</param>
        protected abstract void RegisterDataface(IDataface<T> dataface);
    }
}
