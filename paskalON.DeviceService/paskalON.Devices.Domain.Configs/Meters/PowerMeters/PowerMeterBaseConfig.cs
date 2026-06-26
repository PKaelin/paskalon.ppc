
namespace paskalON.Devices.Domain.Configs.Meters.PowerMeters
{
    /// <summary>
    /// Base class for power meter configurations.
    /// </summary>
    public abstract class PowerMeterBaseConfig : DeviceIdNameBase
    {
        /// <summary>
        /// Relationship to PowerMeterDeviceConfigId.
        /// </summary>
        public int? PowerMeterDeviceConfigId { get; set; }


        /// <summary>
        /// Relationship to PowerMeterDeviceConfig.
        /// </summary>
        public required PowerMeterDeviceConfig PowerMeterDeviceConfig { get; set; }


        /// <summary>
        /// Relationship to ModbusConfigId.
        /// </summary>
        public int? ModbusConfigId { get; set; }


        /// <summary>
        /// Relationship to ModbusConfig.
        /// </summary>
        public ModbusConfig? ModbusConfig { get; set; }


        /// <summary>
        /// Relationship to C37ConfigId.
        /// </summary>
        public int? C37ConfigId { get; set; }


        /// <summary>
        /// Relationship to C37Config.
        /// </summary>
        public C37Config? C37Config { get; set; }


        /// <summary>
        /// Relationship to RedundantPowerMeterConfigId.
        /// </summary>
        public int? RedundantPowerMeterConfigId { get; set; }


        /// <summary>
        /// Relationship to RedundantPowerMeterConfig.
        /// </summary>
        public PowerMeterBaseConfig? RedundantPowerMeterConfig { get; set; }


    }
}
