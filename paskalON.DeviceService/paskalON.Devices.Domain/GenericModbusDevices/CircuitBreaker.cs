using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;

namespace paskalON.Devices.Domain.GenericModbusDevices
{
    public class CircuitBreaker : GenericModbusDevice
    {
        private readonly GenericModbusConfig _config;


        public CircuitBreaker(ILogger logger, GenericModbusConfig config) : base(logger, config)
        {
            _config = config;
        }
    }
}
