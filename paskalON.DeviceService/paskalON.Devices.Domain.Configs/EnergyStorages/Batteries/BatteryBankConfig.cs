
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Configs.EnergyStorages.Batteries
{
    /// <summary>
    /// Battery bank configuration.
    /// </summary>
    public class BatteryBankConfig : ModbusAddressBase
    {
        /// <summary>
        /// Parent relationship to DerUnitConfig Id.
        /// </summary>
        public int? DerUnitConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerUnitConfig.
        /// </summary>
        public DerUnitConfig? DerUnitConfig { get; set; }


        /// <summary>
        /// Relationship to BatteryBankDeviceConfig Id.
        /// </summary>
        public int? BatteryBankDeviceConfigId { get; set; }


        /// <summary>
        /// Relationship to BatteryBankDeviceConfig.
        /// </summary>
        public BatteryBankDeviceConfig? BatteryBankDeviceConfig { get; set; }



        /// <summary>
        /// Gets or sets a flag indicating whether the battery bank is initially connected or not.
        /// </summary>
        public bool InitiallyConnected { get; set; } = true;

    }
}
