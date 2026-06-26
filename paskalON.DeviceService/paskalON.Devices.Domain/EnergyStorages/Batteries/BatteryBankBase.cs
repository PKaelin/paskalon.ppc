using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.EnergyStorages.Batteries
{
    public abstract class BatteryBankBase : DerBase
    {
        private readonly BatteryBankConfig _config;

        public DerBatteryStorageUnit BatteryStorageUnit { get; private set; }


        /// <summary>
        /// Flag whether this instance is in maintenance mode this is when the DER Unit is in maintenance mode.
        /// </summary>
        public bool IsInMaintenanceMode => BatteryStorageUnit.IsInMaintenanceMode;


        protected BatteryBankBase(ILogger logger, BatteryBankConfig config, DerBatteryStorageUnit batteryStorageUnit) : base(logger, config)
        {
            _config = config;
            BatteryStorageUnit = batteryStorageUnit;
        }
    }
}
