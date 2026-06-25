namespace paskalON.Devices.Domain.Configs.Meters.PowerMeters
{
    public class PowerMeterModbusConfig : ModbusAddressBase
    {
        /// <summary>
        /// Relationship to PowerMeterDeviceConfigId.
        /// </summary>
        public int? PowerMeterDeviceConfigId { get; set; }


        /// <summary>
        /// Relationship to PowerMeterDeviceConfig.
        /// </summary>
        public PowerMeterDeviceConfig? PowerMeterDeviceConfig { get; set; }

    }
}
