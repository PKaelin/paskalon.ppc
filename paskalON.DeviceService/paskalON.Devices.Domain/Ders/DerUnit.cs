using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Ders
{
    public class DerUnit : DerBase
    {
        private readonly DerUnitConfig _config;


        /// <summary>
        /// Parent DerCircuit.
        /// </summary>
        public DerCircuit DerCircuit { get; private set; }


        public bool IsInMaintenanceMode { get; set; }


        public DerUnit(ILogger logger, DerUnitConfig config, DerCircuit derCircuit) : base(logger, config)
        {
            _config = config;
            DerCircuit = derCircuit;
        }

    }
}
