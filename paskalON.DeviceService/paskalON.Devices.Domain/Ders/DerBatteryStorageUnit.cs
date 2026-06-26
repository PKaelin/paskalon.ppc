using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.PowerConversionSystems;

namespace paskalON.Devices.Domain.Ders
{
    public class DerBatteryStorageUnit : DerUnit
    {
        private readonly DerBatteryStorageUnitConfig _config;


        public PowerConversionSystemBase? PowerConversionSystem { get; set; }

        public List<BatteryBankBase> BatteryBanks { get; set; } = new List<BatteryBankBase>();


        public DerBatteryStorageUnit(ILogger logger, DerBatteryStorageUnitConfig config, DerCircuit derCircuit) : base(logger, config, derCircuit)
        {
            _config = config;
        }
    }
}
