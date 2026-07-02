using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.GenericModbusDevices.Entries;

namespace paskalON.Devices.Domain.GenericModbusDevices
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Generic Modbus Device that communicates using the universal Modbus Protocol.
    /// </summary>
    public class GenericModbusDevice : GenericModbusDeviceBase
    {
        /// <summary>
        /// Generic Modbus configuration.
        /// </summary>
        private readonly GenericModbusConfig _config;


        /// <summary>
        /// Constructor of <see cref="GenericModbusDevice"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The generic Modbus configuration.</param>
        /// <param name="genericModbusEntries">List of generic Modbus entries.</param>
        /// <param name="device">The device interface.</param>
        public GenericModbusDevice(ILogger logger, GenericModbusConfig config, List<GenericModbusEntryBase> genericModbusEntries, IGenericModbusDevice<GenericModbusDevice> device)
            : base(logger, config, genericModbusEntries, (IGenericModbusDevice<GenericModbusDeviceBase>)device)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }
    }
}
