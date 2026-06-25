
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Configs
{
    /// <summary>
    /// Root database context configuration class for the all devices and structure.
    /// </summary>
    public class DeviceConfig
    {
        /// <summary>
        /// Root instance of device configurations.
        /// </summary>
        public required DerConfig DerConfig { get; set; }
    }
}
