using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.GenericModbusDevices;

namespace paskalON.Devices.Domain.Ders
{
    public class DerCircuit : DerBase
    {
        private readonly DerCircuitConfig _config;

        /// <summary>
        /// Parent DerGroup.
        /// </summary>
        public DerGroup DerGroup { get; private set; }


        /// <summary>
        /// List of DER units.
        /// </summary>
        public List<DerUnit> DerUnits { get; set; } = new List<DerUnit>();


        public CircuitBreaker? CircuitBreaker { get; set; }


        public DerCircuit(ILogger logger, DerCircuitConfig config, DerGroup derGroup) : base(logger, config)
        {
            _config = config;
            DerGroup = derGroup;
        }
    }
}
