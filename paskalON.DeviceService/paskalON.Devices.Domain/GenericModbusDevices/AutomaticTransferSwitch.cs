using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;

namespace paskalON.Devices.Domain.GenericModbusDevices
{
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
        public AutomaticTransferSwitch(ILogger logger, AutomaticTransferSwitchConfig config) : base(logger, config)
        {
            _config = config;
        }
    }
}
