using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.GenericModbusDevices;
using paskalON.Devices.Domain.PowerMeters;

namespace paskalON.Devices.Domain.Ders
{
    /// <summary>
    /// Distributed Energy Resource. Root for all DERs. 
    /// </summary>
    public class Der : DerBase
    {
        /// <summary>
        /// Der configuration.
        /// </summary>
        private readonly DerConfig _config;


        /// <summary>
        /// List of DER groups that can be split up onto different device service services 
        /// and therefore run on different machines.
        /// </summary>
        public List<DerGroup> DerGroups { get; set; } = new List<DerGroup>();


        /// <summary>
        /// List of generic Modbus devices.
        /// </summary>
        public List<GenericModbusDevice> GenericModbusDevices { get; set; } = new List<GenericModbusDevice>();


        /// <summary>
        /// List of system meters. Usually there is one with redundancy within.
        /// </summary>
        public List<SystemPowerMeter> SystemPowerMeters { get; set; } = new List<SystemPowerMeter>();


        /// <summary>
        /// List of auxiliary power meters.
        /// </summary>
        public List<AuxiliaryPowerMeter> AuxiliaryPowerMeters { get; set; } = new List<AuxiliaryPowerMeter>();


        /// <summary>
        /// List of external power meters.
        /// </summary>
        public List<ExternalPowerMeter> ExternalPowerMeters { get; set; } = new List<ExternalPowerMeter>();




        public Der(ILogger logger, DerConfig config) : base(logger, config)
        {
            _config = config;
        }
    }
}
