
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    /// <summary>
    /// Generic Modbus configuration.
    /// </summary>
    public class GenericModbusConfig : GenericModbusBaseConfig
    {
        /// <summary>
        /// Parent relationship to DerConfig Id.
        /// </summary>
        public int DerConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerConfig.
        /// </summary>
        public required DerConfig DerConfig { get; set; }


        /// <summary>
        /// Relationship to GenericModbusDeviceConfig Id.
        /// </summary>
        public int GenericModbusDeviceConfigId { get; set; }


        /// <summary>
        /// Relationship to GenericModbusDeviceConfig.
        /// </summary>
        public required GenericModbusDeviceConfig GenericModbusDeviceConfig { get; set; }

    }
}
