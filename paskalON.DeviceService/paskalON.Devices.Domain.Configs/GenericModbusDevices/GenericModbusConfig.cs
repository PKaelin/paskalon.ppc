
namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    /// <summary>
    /// Generic Modbus configuration.
    /// </summary>
    public class GenericModbusConfig : GenericModbusBaseConfig
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
