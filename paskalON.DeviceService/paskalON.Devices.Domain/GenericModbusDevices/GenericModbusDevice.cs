
using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.GenericModbusDevices
{
    public class GenericModbusDevice : DerBase
    {
        private readonly GenericModbusConfig _config;


        public GenericModbusDevice(ILogger logger, GenericModbusConfig config) : base(logger, config)
        {
            _config = config;
        }
    }
}
