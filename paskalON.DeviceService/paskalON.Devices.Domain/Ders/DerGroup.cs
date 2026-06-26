using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Ders
{
    /// <summary>
    /// DER group that can be split up onto different device service services 
    /// and therefore run on different machines.
    /// </summary>
    public class DerGroup : DerBase
    {
        private readonly DerGroupConfig _config;


        /// <summary>
        /// Parent Distributed Energy Resource (DER).
        /// </summary>
        public Der Der { get; private set; }


        /// <summary>
        /// List of DERs that are grouped in a circuit.
        /// A circuit can have a breaker and a meter
        /// </summary>
        public List<DerCircuit> DerCircuits { get; set; } = new List<DerCircuit>();


        public DerGroup(ILogger logger, DerGroupConfig config, Der der) : base(logger, config)
        {
            _config = config;
            Der = der;
        }
    }
}
