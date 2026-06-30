using paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    public class AutomaticTransferSwitchDeviceConfig : NameBase
    {
        /// <summary>
        /// Relationship to GenericModbusMapConfigId.
        /// </summary>
        public int? GenericModbusMapConfigId { get; set; }


        /// <summary>
        /// Relationship to GenericModbusMapConfig.
        /// </summary>
        public GenericModbusMapConfig? GenericModbusMapConfig { get; set; }



        /// <summary>
        /// The class name of the type to instantiate.
        /// This uniquely identifies the eventually used type of component. (e.g. a ManufacturerPcs, ManufacturerBattery etc.).
        /// </summary>
        public required string ClassName { get; set; }


        public GenericModbusCoilMapEntryConfig? GridConnected { get; set; }

        public GenericModbusCoilMapEntryConfig? BackupConnected { get; set; }

    }
}
