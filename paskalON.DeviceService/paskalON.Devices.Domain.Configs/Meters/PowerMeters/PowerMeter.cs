
namespace paskalON.Devices.Domain.Configs.Meters.PowerMeters
{
    /// <summary>
    /// Base class for all power meters.
    /// </summary>
    public abstract class PowerMeter : NameBase
    {
        /// <summary>
        /// Relationship to PowerMeterModbusConfigId.
        /// </summary>
        public int? PowerMeterModbusConfigId { get; set; }


        /// <summary>
        /// Relationship to PowerMeterModbusConfig.
        /// </summary>
        public PowerMeterModbusConfig? PowerMeterModbusConfig { get; set; }


        /// <summary>
        /// Relationship to PowerMeterC37ConfigId.
        /// </summary>
        public int? PowerMeterC37ConfigId { get; set; }


        /// <summary>
        /// Relationship to PowerMeterC37Config.
        /// </summary>
        public PowerMeterC37Config? PowerMeterC37Config { get; set; }



        /// <summary>
        /// Relationship to RedundantPowerMeterModbusConfigId.
        /// </summary>
        public int? RedundantPowerMeterModbusConfigId { get; set; }


        /// <summary>
        /// Relationship to RedundantPowerMeterModbusConfig.
        /// </summary>
        public PowerMeterModbusConfig? RedundantPowerMeterModbusConfig { get; set; }


        /// <summary>
        /// Relationship to RedundantPowerMeterC37ConfigId.
        /// </summary>
        public int? RedundantPowerMeterC37ConfigId { get; set; }


        /// <summary>
        /// Relationship to PowerMeterC37Config.
        /// </summary>
        public PowerMeterC37Config? RedundantPowerMeterC37Config { get; set; }
    }
}
