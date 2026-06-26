

using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.Meters.PowerMeters
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Power meter base call for all power meters.
    /// </summary>
    /// <remarks>
    /// A power meter measures electrical values (e.g., active/reactive power in watts/vars, voltage, frequency, and power factor).
    /// </remarks>
    public abstract class PowerMeterBase : DerBase
    {
        /// <summary>
        /// Power meter configuration.
        /// </summary>
        private readonly Configs.Meters.PowerMeters.PowerMeterBaseConfig _powerMeterConfig;


        /// <summary>
        /// This lock object needs to be used by this class and derived classes.
        /// The classes need to use the same lock for thread safety.
        /// </summary>
        protected object dataLock = new();


        /// <summary>
        /// Returns true if a communication error has occurred.
        /// </summary>
        public bool CommunicationError { get; set; }




        /// <summary>
        /// Constructor of <see cref="PowerMeterBase"/>
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="powerMeterConfig">The power meter configuration.</param>
        public PowerMeterBase(ILogger logger, Configs.Meters.PowerMeters.PowerMeterBaseConfig powerMeterConfig) : base(logger, powerMeterConfig)
        {
            _powerMeterConfig = powerMeterConfig;
        }
    }
}
