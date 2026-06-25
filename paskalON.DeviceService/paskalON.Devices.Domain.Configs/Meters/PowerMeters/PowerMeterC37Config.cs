namespace paskalON.Devices.Domain.Configs.Meters.PowerMeters
{
    public class PowerMeterC37Config : C37Base
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
