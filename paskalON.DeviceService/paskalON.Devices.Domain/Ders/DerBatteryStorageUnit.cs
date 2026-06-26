using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.PowerConversionSystems;

namespace paskalON.Devices.Domain.Ders
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// DER battery storage unit for one or multiple battery banks and one power conversion system.
    /// </summary>
    public class DerBatteryStorageUnit : DerUnit
    {
        /// <summary>
        /// DER battery storage unit configuration.
        /// </summary>
        private readonly DerBatteryStorageUnitConfig _config;


        /// <summary>
        /// Power conversion system for this battery storage unit.
        /// </summary>
        public PowerConversionSystemBase? PowerConversionSystem { get; set; }


        /// <summary>
        /// One or many battery banks for this battery storage unit.
        /// </summary>
        public List<BatteryBankBase> BatteryBanks { get; set; } = new List<BatteryBankBase>();


        /// <summary>
        /// Include operations sent to parent or PCS in the BatteryStorageUnits.
        /// Default this to true; almost all BatteryStorageUnits will want to behave this way.
        /// </summary>
        public bool IncludeBatteryInOperations { get => _config.IncludeBatteryInOperations; }


        /// <summary>
        /// Constructor of <see cref="DerBatteryStorageUnit"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The DER battery storage unit configuration.</param>
        /// <param name="derCircuit">The parent DER circuit.</param>
        public DerBatteryStorageUnit(ILogger logger, DerBatteryStorageUnitConfig config, DerCircuit derCircuit) : base(logger, config, derCircuit)
        {
            _config = config;
        }
    }
}
