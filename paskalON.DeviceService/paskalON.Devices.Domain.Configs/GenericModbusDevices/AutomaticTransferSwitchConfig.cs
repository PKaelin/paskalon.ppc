
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    /// <summary>
    /// An automatic transfer switch (ATS) is a device that automatically transfers power from 
    /// a primary source to a backup source in the event of a failure.
    /// </summary>
    public class AutomaticTransferSwitchConfig : GenericModbusBaseConfig
    {
        /// <summary>
        /// Parent relationship to DerConfig Id.
        /// </summary>
        public int DerConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerConfig.
        /// </summary>
        public required DerConfig DerConfig { get; set; }


        /// <summary>
        /// Relationship to AutomaticTransferSwitchDeviceConfigId.
        /// </summary>
        public int AutomaticTransferSwitchDeviceConfigId { get; set; }


        /// <summary>
        /// Relationship to AutomaticTransferSwitchDeviceConfig.
        /// </summary>
        public required AutomaticTransferSwitchDeviceConfig AutomaticTransferSwitchDeviceConfig { get; set; }

    }
}
