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
        public int DerGroupConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerGroupConfig.
        /// </summary>
        public required DerGroupConfig DerGroupConfig { get; set; }


        /// <summary>
        /// Child relationship to a list of DerUnitConfig.
        /// </summary>
        public ICollection<DerUnitConfig> DerUnitConfigs { get; set; } = [];


        /// <summary>
        /// Relationship to CircuitBreakerConfig.
        /// DER circuit could have a circuit breaker.
        /// </summary>
        public CircuitBreakerConfig? CircuitBreakerConfig { get; set; }


        /// <summary>
        /// Relationship to CircuitPowerMeterConfig.
        /// DER circuit could have a power meter config those circuits can be also called feeder power meters.
        /// </summary>
        public CircuitPowerMeterConfig? CircuitPowerMeterConfig { get; set; }

    }
}
