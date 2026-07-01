using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs;
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
        /// Metrics publisher for publishing metrics related to the DER device.
        /// </summary>
        protected readonly IMetricsPublisher<T> _metricsPublisher;


        /// <summary>
        /// Constructor of <see cref="DerDeviceBase{T}"/>
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="nameBase">The name base configuration.</param>
        /// <param name="metricsPublisher">The metrics publisher.</param>
        protected DerDeviceBase(ILogger logger, NameBase nameBase, IMetricsPublisher<T> metricsPublisher) : base(logger, nameBase)
        {
            ArgumentNullException.ThrowIfNull(metricsPublisher);

            _metricsPublisher = metricsPublisher;
        }
    }
}
