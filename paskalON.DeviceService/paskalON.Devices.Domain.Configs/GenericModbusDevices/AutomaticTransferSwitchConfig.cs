
namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    /// <summary>
    /// An automatic transfer switch (ATS) is a device that automatically transfers power from 
    /// a primary source to a backup source in the event of a failure.
    /// </summary>
    public class AutomaticTransferSwitchConfig : ModbusAddressBase
    {
        /// <summary>
        /// Relationship to AutomaticTransferSwitchDeviceConfigId.
        /// </summary>
        public int? AutomaticTransferSwitchDeviceConfigId { get; set; }


        /// <summary>
        /// Relationship to AutomaticTransferSwitchDeviceConfig.
        /// </summary>
        public AutomaticTransferSwitchDeviceConfig? AutomaticTransferSwitchDeviceConfig { get; set; }

    }
}
