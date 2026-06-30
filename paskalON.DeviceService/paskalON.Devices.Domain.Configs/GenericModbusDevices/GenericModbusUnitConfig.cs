using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    public class GenericModbusUnitConfig : GenericModbusBaseConfig
    {
        /// <summary>
        /// Parent relationship to DerUnitConfig Id.
        /// </summary>
        public int DerUnitConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerUnitConfig.
        /// </summary>
        public required DerUnitConfig DerUnitConfig { get; set; }



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