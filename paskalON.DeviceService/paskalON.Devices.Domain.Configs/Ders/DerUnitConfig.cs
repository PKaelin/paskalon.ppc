using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;

namespace paskalON.Devices.Domain.Configs.Ders
{
    /// <summary>
    /// A base class for <see cref="DerBatteryStorageUnitConfig"/> and <see cref="DerSolarUnitConfig"/> containing common functionality.
    /// </summary>
    public abstract class DerUnitConfig : NameBase
    {
        /// <summary>
        /// Parent relationship to DerCircuitConfig Id.
        /// </summary>
        public int DerCircuitConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerCircuitConfig.
        /// </summary>
        public required DerCircuitConfig DerCircuitConfig { get; set; }


        /// <summary>
        /// Child relationship to PowerConversionSystemConfig.
        /// </summary>
        /// <remarks>
        /// Ono to one configurations are not possible with data annotations use fluent API in OnModelCreating.
        /// </remarks>
        public PowerConversionSystemConfig PowerConversionSystemConfig { get; set; } = null!;


        /// <summary>
        /// Relationship to DerContainerConfig.
        /// DER unit could be in a container that has GMDs, HAVAC, UPS, etc.
        /// </summary>
        public DerContainerConfig? DerContainerConfig { get; set; }



        /// <summary>
        /// Relationship to GenericModbusUnitConfig.
        /// DER unit can have a generic Modbus device.
        /// </summary>
        public GenericModbusUnitConfig? GenericModbusUnitConfig { get; set; }



        /// <summary>
        /// Indicates whether or not the DER Unit is in maintenance mode.
        /// </summary>
        public bool InMaintenanceMode { get; set; } = false;

    }
}
