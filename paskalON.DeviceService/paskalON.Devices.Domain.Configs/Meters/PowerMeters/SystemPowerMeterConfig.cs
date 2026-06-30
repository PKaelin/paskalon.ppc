using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Configs.Meters.PowerMeters
{
    /// <summary>
    /// System power meter could be either a C37 or a Modbus meter but not both.
    /// Redundant meter must be different Id than this.
    /// Make sure that if C37 or Modbus properties are set the device has the appropriate configuration.
    /// </summary>
    public class SystemPowerMeterConfig : PowerMeterBaseConfig
    {
        /// <summary>
        /// Parent relationship to DerConfig Id.
        /// </summary>
        public int DerConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerConfig.
        /// </summary>
        public required DerConfig DerConfig { get; set; }
    }
}
