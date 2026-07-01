using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.GenericModbusDevices.Entries;
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


        public required List<GenericModbusEntryBase> GenericModbusEntries { get; set; }


        /// <summary>
        /// Constructor of <see cref="GenericModbusDeviceBase"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The generic Modbus configuration.</param>
        /// <param name="metricsPublisher">Metrics publisher interface.</param>
        public GenericModbusDeviceBase(ILogger logger, GenericModbusBaseConfig config, List<GenericModbusEntryBase> genericModbusEntries, IMetricsPublisher<GenericModbusDeviceBase> metricsPublisher)
            : base(logger, config, metricsPublisher)
        {
            _config = config;
            RegisterMetrics();
        }


        /// <summary>
        /// Register base metrics with the metrics publisher.
        /// </summary>
        private void RegisterMetrics()
        {
            foreach (GenericModbusPointEntry entry in GenericModbusEntries.OfType<GenericModbusPointEntry>())
            {
                _metricsPublisher.Register<byte>(entry.Name, x => entry.Value, _config.MetricsFactorClass1);
            }

            foreach (GenericModbusRegisterEntry entry in GenericModbusEntries.OfType<GenericModbusRegisterEntry>())
            {
                _metricsPublisher.Register<Int16>(entry.Name, x => entry.Value, _config.MetricsFactorClass1);
            }
        }
    }
}
