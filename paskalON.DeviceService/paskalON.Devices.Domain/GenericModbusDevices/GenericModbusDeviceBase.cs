using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.GenericModbusDevices
{
    /// <summary>
    /// Base class for generic Modbus devices.
    /// </summary>
    public abstract class GenericModbusDeviceBase : DerBase
    {
        /// <summary>
        /// Generic Modbus base configuration.
        /// </summary>
        private readonly GenericModbusBaseConfig _config;


        /// <summary>
        /// Constructor of <see cref="GenericModbusDeviceBase"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The generic Modbus configuration.</param>
        public GenericModbusDeviceBase(ILogger logger, GenericModbusBaseConfig config) : base(logger, config)
        {
            _config = config;
        }
    }
}
