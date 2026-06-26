using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.GenericModbusDevices;
using paskalON.Devices.Domain.Meters.PowerMeters;

namespace paskalON.Devices.Domain.Ders
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// DER circuit is a physical grouping of DERs.
    /// A circuit can have a circuit breaker and a circuit meter (sometimes called feeder meter).
    /// </summary>
    public class DerCircuit : DerBase
    {
        /// <summary>
        /// DER circuit configuration.
        /// </summary>
        private readonly DerCircuitConfig _config;


        /// <summary>
        /// Parent DerGroup.
        /// </summary>
        public DerGroup DerGroup { get; private set; }


        /// <summary>
        /// List of DER units.
        /// </summary>
        public List<DerUnit> DerUnits { get; set; } = new List<DerUnit>();


        /// <summary>
        /// Optional circuit breaker.
        /// </summary>
        /// <remarks>
        /// Circuit breaker is a safety threshold that automatically pauses or stops the circuit when a predefined
        /// limit is reached. It acts as a fail-safe 
        /// </remarks>
        public CircuitBreaker? CircuitBreaker { get; set; }


        /// <summary>
        /// Optional circuit power meter.
        /// </summary>
        /// <remarks>
        /// Circuit power meter is a power meter just for this specific circuit.
        /// It is sometimes called feeder meter.
        /// </remarks>
        public CircuitPowerMeter? CircuitPowerMeter { get; set; }



        /// <summary>
        /// Constructor of <see cref="DerCircuit"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The DER circuit configuration.</param>
        /// <param name="derGroup">The parent DER group.</param>
        public DerCircuit(ILogger logger, DerCircuitConfig config, DerGroup derGroup) : base(logger, config)
        {
            _config = config;
            DerGroup = derGroup;
        }
    }
}
