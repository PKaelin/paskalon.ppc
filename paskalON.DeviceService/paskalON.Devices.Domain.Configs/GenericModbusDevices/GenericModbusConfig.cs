
namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    /// <summary>
    /// Generic Modbus configuration.
    /// </summary>
    public class GenericModbusConfig : ModbusAddressBase
    {
        /// <summary>
        /// Relationship to GenericModbusDeviceConfig Id.
        /// </summary>
        public int? GenericModbusDeviceConfigId { get; set; }


        /// <summary>
        /// Relationship to GenericModbusDeviceConfig.
        /// </summary>
        public GenericModbusDeviceConfig? GenericModbusDeviceConfig { get; set; }

    }
}
