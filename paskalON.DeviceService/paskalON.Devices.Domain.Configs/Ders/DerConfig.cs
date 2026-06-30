using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;

namespace paskalON.Devices.Domain.Configs.Ders
{
    /// <summary>
    /// DER configuration. Root for all DER configurations.
    /// Configuration has a name which usually it the project or customer name.
    /// </summary>
    public class DerConfig : NameBase
    {
        /// <summary>
        /// List of DER groups that can be split up onto different device service services 
        /// and therefore run on different machines.
        /// </summary>
        public ICollection<DerGroupConfig> DerGroupConfigs { get; set; } = [];


        /// <summary>
        /// List of generic Modbus devices.
        /// </summary>
        public ICollection<GenericModbusConfig> GenericModbusConfigs { get; set; } = [];


        /// <summary>
        /// List of automatic transfer switches.
        /// </summary>
        public ICollection<AutomaticTransferSwitchConfig> AutomaticTransferSwitchConfigs { get; set; } = [];


        /// <summary>
        /// List of system meters. Usually there is one with redundancy within.
        /// </summary>
        public ICollection<SystemPowerMeterConfig> SystemPowerMeterConfigs { get; set; } = [];


        /// <summary>
        /// List of auxiliary power meters.
        /// </summary>
        public ICollection<AuxiliaryPowerMeterConfig> AuxiliaryPowerMeterConfigs { get; set; } = [];


        /// <summary>
        /// List of external power meters.
        /// </summary>
        public ICollection<ExternalPowerMeterConfig> ExternalPowerMeterConfigs { get; set; } = [];

    }
}
