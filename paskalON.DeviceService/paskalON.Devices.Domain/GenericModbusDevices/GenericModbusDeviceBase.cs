using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.GenericModbusDevices.Entries;
using paskalON.Domains.Contracts;
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
        /// Generic Modbus device instance that communicates with the device.
        /// </summary>
        private readonly IGenericModbusDevice<GenericModbusDeviceBase> _device;


        /// <summary>
        /// List of generic Modbus entries that represent the data points and registers of the device.
        /// </summary>
        public required List<GenericModbusEntryBase> GenericModbusEntries { get; set; }


        /// <summary>
        /// Constructor of <see cref="GenericModbusDeviceBase"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The generic Modbus configuration.</param>
        /// <param name="device">The device interface.</param>
        public GenericModbusDeviceBase(ILogger logger, GenericModbusBaseConfig config, List<GenericModbusEntryBase> genericModbusEntries, IGenericModbusDevice<GenericModbusDeviceBase> device)
            : base(logger, config, device)
        {
            _config = config;
            _device = device;
            RegisterMetrics(device.MetricsPublisher);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void RegisterMetrics(IMetricsPublisher<GenericModbusDeviceBase> metricsPublisher)
        {
            foreach (GenericModbusPointEntry entry in GenericModbusEntries.OfType<GenericModbusPointEntry>())
            {
                metricsPublisher.Register<byte>(entry.Name, x => entry.Value, _config.MetricsFactorClass1);
            }

            foreach (GenericModbusRegisterEntry entry in GenericModbusEntries.OfType<GenericModbusRegisterEntry>())
            {
                metricsPublisher.Register<Int16>(entry.Name, x => entry.Value, _config.MetricsFactorClass1);
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void RegisterDataface(IDataface<GenericModbusDeviceBase> dataface)
        {
            // TODO:
        }
    }
}
