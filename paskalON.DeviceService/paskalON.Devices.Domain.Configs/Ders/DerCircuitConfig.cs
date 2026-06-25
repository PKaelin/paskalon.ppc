using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;

namespace paskalON.Devices.Domain.Configs.Ders
{
    /// <summary>
    /// DER circuit is a physical grouping of devices that can be protected by a breaker
    /// and have a separate power meter.
    /// </summary>
    public class DerCircuitConfig : NameBase
    {
        /// <summary>
        /// Parent relationship to DerGroupConfig Id.
        /// </summary>
        public int? DerGroupConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerGroupConfig.
        /// </summary>
        public DerGroupConfig? DerGroupConfig { get; set; }


        /// <summary>
        /// Child relationship to a list of DerSolarUnitConfig.
        /// </summary>
        public List<DerSolarUnitConfig>? DerSolarUnitConfigs { get; set; }


        /// <summary>
        /// Child relationship to a list of DerBatteryStorageUnitConfig .
        /// </summary>
        public List<DerBatteryStorageUnitConfig>? DerBatteryStorageUnitConfigs { get; set; }


        /// <summary>
        /// Relationship to CircuitBreakerConfig Id.
        /// DER circuit could have a circuit breaker.
        /// </summary>
        public int? CircuitBreakerConfigId { get; set; }


        /// <summary>
        /// Relationship to CircuitBreakerConfig.
        /// DER circuit could have a circuit breaker.
        /// </summary>
        public CircuitBreakerConfig? CircuitBreakerConfig { get; set; }


        /// <summary>
        /// Relationship to CircuitPowerMeterConfig Id.
        /// DER circuit could have a power meter config those circuits can be also called feeder power meters.
        /// </summary>
        public int? CircuitPowerMeterConfigId { get; set; }


        /// <summary>
        /// Relationship to CircuitPowerMeterConfig.
        /// DER circuit could have a power meter config those circuits can be also called feeder power meters.
        /// </summary>
        public CircuitPowerMeterConfig? CircuitPowerMeterConfig { get; set; }

    }
}
