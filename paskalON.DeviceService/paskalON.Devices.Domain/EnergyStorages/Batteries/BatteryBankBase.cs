using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.EnergyStorages.Batteries
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Battery bank base for the physical battery bank.
    /// </summary>
    /// <remarks>
    /// Battery bank is a collection of individual batteries wired together in series or parallel to function as a single large-scale energy storage.
    /// </remarks>
    public abstract class BatteryBankBase : DerBase
    {
        /// <summary>
        /// Battery bank configuration.
        /// </summary>
        private readonly BatteryBankConfig _config;


        /// <summary>
        /// This lock object needs to be used by this class and derived classes.
        /// The classes need to use the same lock for thread safety.
        /// </summary>
        protected object dataLock = new();


        /// <summary>
        /// Parent battery storage unit.
        /// </summary>
        public DerBatteryStorageUnit BatteryStorageUnit { get; private set; }


        /// <summary>
        /// Returns true if a communication error has occurred.
        /// </summary>
        public bool CommunicationError { get; set; }


        /// <summary>
        /// Flag whether this instance is in maintenance mode this is when the DER Unit is in maintenance mode.
        /// </summary>
        public bool IsInMaintenanceMode { get => BatteryStorageUnit.IsInMaintenanceMode; }


        /// <summary>
        /// Gets or sets a flag indicating whether the battery bank is initially connected or not.
        /// </summary>
        public bool InitiallyConnected { get => _config.InitiallyConnected; }


        /// <summary>
        /// Constructor of <see cref="BatteryBankBase"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The battery bank configuration.</param>
        /// <param name="batteryStorageUnit">The paren battery storage unit.</param>
        protected BatteryBankBase(ILogger logger, BatteryBankConfig config, DerBatteryStorageUnit batteryStorageUnit) : base(logger, config)
        {
            _config = config;
            BatteryStorageUnit = batteryStorageUnit;
        }
    }
}
