using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;

namespace paskalON.Devices.Domain.Configs.Ders
{
    /// <summary>
    /// Battery storage DER unit configuration.
    /// </summary>
    public class DerBatteryStorageUnitConfig : DerUnitConfig
    {

        /// <summary>
        /// Child relationship to PowerConversionSystemConfig.
        /// </summary>
        public ICollection<BatteryBankConfig> BatteryBankConfigs { get; set; } = [];



        /// <summary>
        /// Include operations sent to parent or PCS in the BatteryStorageUnits.
        /// Default this to true; almost all BatteryStorageUnits will want to behave this way.
        /// </summary>
        public bool IncludeBatteryInOperations { get; set; } = true;

    }
}
