
namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    /// <summary>
    /// Circuit Breaker generic Modbus device configuration
    /// </summary>
    public class CircuitBreakerConfig : ModbusAddressBase
    {
        /// <summary>
        /// Relationship to CircuitBreakerDeviceConfig Id.
        /// </summary>
        public int? CircuitBreakerDeviceConfigId { get; set; }


        /// <summary>
        /// Relationship to CircuitBreakerDeviceConfig.
        /// </summary>
        public CircuitBreakerDeviceConfig? CircuitBreakerDeviceConfig { get; set; }
    }
}