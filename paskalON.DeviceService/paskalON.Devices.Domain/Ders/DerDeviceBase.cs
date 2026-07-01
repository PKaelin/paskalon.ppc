using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs;
using paskalON.Domains.Telemetry;

namespace paskalON.Devices.Domain.Ders
{
    public abstract class DerDeviceBase<T> : DerBase
    {
        protected readonly IMetricsPublisher<T> _metricsPublisher;

        protected DerDeviceBase(ILogger logger, NameBase nameBase, IMetricsPublisher<T> metricsPublisher) : base(logger, nameBase)
        {
            _metricsPublisher = metricsPublisher;
        }
    }
}
