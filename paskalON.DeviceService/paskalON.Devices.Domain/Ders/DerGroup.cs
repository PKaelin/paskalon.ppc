using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Ders
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// DER group that can be split up onto different device service services and therefore run on different machines.
    /// </summary>
    /// <remarks>
    /// Used to logically split up a large system into smaller chunks so it can be distributed over physical machines.
    /// </remarks>
    public class DerGroup : DerBase
    {
        /// <summary>
        /// DER Group configuration.
        /// </summary>
        private readonly DerGroupConfig _config;


        /// <summary>
        /// Parent Distributed Energy Resource (DER).
        /// </summary>
        public Der Der { get; private set; }


        /// <summary>
        /// List of DERs that are grouped in a circuit.
        /// A circuit can have a breaker and a meter.
        /// </summary>
        public List<DerCircuit> DerCircuits { get; set; } = new List<DerCircuit>();


        /// <summary>
        /// Constructor of <see cref="DerGroup"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The DER group configuration.</param>
        /// <param name="der">The parent DER.</param>
        public DerGroup(ILogger logger, DerGroupConfig config, Der der) : base(logger, config)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(der);

            _config = config;
            Der = der;
        }
    }
}
