using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Domains.Telemetry;

namespace paskalON.Devices.Domain.GenericModbusDevices
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// An Automatic Transfer Switch (ATS) is an intelligent, self-acting device that shifts an electrical
    /// load between two power sources without requiring human intervention.
    /// </summary>
    public class AutomaticTransferSwitch : GenericModbusDeviceBase
    {
        /// <summary>
        /// Automatic transfer switch configuration.
        /// </summary>
        private readonly AutomaticTransferSwitchConfig _config;


        /// <summary>
        /// Constructor of <see cref="AutomaticTransferSwitch"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The automatic transfer switch configuration.</param>
        /// <param name="metricsPublisher">Metrics publisher interface.</param>
        public AutomaticTransferSwitch(ILogger logger, AutomaticTransferSwitchConfig config, IMetricsPublisher<AutomaticTransferSwitch> metricsPublisher)
            : base(logger, config, (IMetricsPublisher<GenericModbusDeviceBase>)metricsPublisher)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }
    }
}
