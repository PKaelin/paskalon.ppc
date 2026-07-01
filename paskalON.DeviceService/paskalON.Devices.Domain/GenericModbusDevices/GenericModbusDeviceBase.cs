using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Ders;
using paskalON.Domains.Telemetry;

namespace paskalON.Devices.Domain.GenericModbusDevices
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Base class for generic Modbus devices.
    /// </summary>
    public abstract class GenericModbusDeviceBase : DerDeviceBase<GenericModbusDeviceBase>
    {
        /// <summary>
        /// Generic Modbus base configuration.
        /// </summary>
        /// <remarks>
        /// Inherits from ModbusConfig.
        /// </remarks>
        private readonly GenericModbusBaseConfig _config;


        /// <summary>
        /// Constructor of <see cref="GenericModbusDeviceBase"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The generic Modbus configuration.</param>
        /// <param name="metricsPublisher">Metrics publisher interface.</param>
        public GenericModbusDeviceBase(ILogger logger, GenericModbusBaseConfig config, IMetricsPublisher<GenericModbusDeviceBase> metricsPublisher)
            : base(logger, config, metricsPublisher)
        {
            _config = config;
        }
    }
}
