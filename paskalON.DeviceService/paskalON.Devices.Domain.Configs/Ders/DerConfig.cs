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
        public List<DerGroupConfig>? DerGroupConfigs { get; set; }


        /// <summary>
        /// List of generic Modbus devices.
        /// </summary>
        public List<GenericModbusConfig>? GenericModbusConfigs { get; set; }


        /// <summary>
        /// List of system meters. Usually there is one with redundancy within.
        /// </summary>
        public List<SystemPowerMeterConfig>? SystemPowerMeterConfigs { get; set; }


        /// <summary>
        /// List of auxiliary power meters.
        /// </summary>
        public List<AuxiliaryPowerMeterConfig>? AuxiliaryPowerMeterConfigs { get; set; }


        /// <summary>
        /// List of external power meters.
        /// </summary>
        public List<ExternalPowerMeterConfig>? ExternalPowerMeterConfigs { get; set; }

    }
}
