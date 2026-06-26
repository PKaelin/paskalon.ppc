using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Ders
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// DER unit is the base unit for all unit types like <see cref="DerSolarUnit"/> or <see cref="DerBatteryStorageUnit"/>.
    /// </summary>
    public abstract class DerUnit : DerBase
    {
        /// <summary>
        /// DER unit configuration.
        /// </summary>
        private readonly DerUnitConfig _config;


        /// <summary>
        /// Parent DerCircuit.
        /// </summary>
        public DerCircuit DerCircuit { get; private set; }


        /// <summary>
        /// Flag whether this unit is in maintenance mode.
        /// </summary>
        /// <remarks>
        /// Single devices dont get set into maintenance mode. The unit does.
        /// </remarks>
        public bool IsInMaintenanceMode { get; set; }


        /// <summary>
        /// Constructor of <see cref="DerUnit"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The DER unit configuration.</param>
        /// <param name="derCircuit">The paren DER circuit.</param>
        public DerUnit(ILogger logger, DerUnitConfig config, DerCircuit derCircuit) : base(logger, config)
        {
            _config = config;
            DerCircuit = derCircuit;
            // Use configured maintenance mode when created.
            IsInMaintenanceMode = _config.InMaintenanceMode;
        }

    }
}
