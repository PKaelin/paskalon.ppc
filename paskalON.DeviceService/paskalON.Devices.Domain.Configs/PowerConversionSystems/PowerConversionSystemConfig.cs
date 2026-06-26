
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Configs.PowerConversionSystems
{
    /// <summary>
    /// Power Conversion System configuration inheriting Modbus device configuration.
    /// </summary>
    public class PowerConversionSystemConfig : DeviceIdNameBase
    {
        /// <summary>
        /// Parent relationship to DerUnitConfig Id.
        /// </summary>
        public int? DerUnitConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerUnitConfig
        /// </summary>
        public required DerUnitConfig DerUnitConfig { get; set; }


        /// <summary>
        /// Relationship tp PowerConversionSystemDeviceConfig Id.
        /// </summary>
        public int? PowerConversionSystemDeviceConfigId { get; set; }

        /// <summary>
        /// Relationship tp PowerConversionSystemDeviceConfig.
        /// </summary>
        public required PowerConversionSystemDeviceConfig PowerConversionSystemDeviceConfig { get; set; }


        /// <summary>
        /// Relationship to ModbusConfig Id.
        /// </summary>
        public int? ModbusConfigId { get; set; }


        /// <summary>
        /// Relationship to ModbusConfig Id.
        /// </summary>
        public ModbusConfig? ModbusConfig { get; set; }



        /// <summary>
        /// Flag whether this PCS is initially started or not.
        /// </summary>
        public bool InitiallyStarted { get; set; } = true;

    }
}
