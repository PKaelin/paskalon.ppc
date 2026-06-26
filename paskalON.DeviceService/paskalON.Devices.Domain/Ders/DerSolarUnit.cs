using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.EnergyResources.Solars;
using paskalON.Devices.Domain.PowerConversionSystems;

namespace paskalON.Devices.Domain.Ders
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Solar unit or solar array for one or multiple solar panels and one power conversion system.
    /// </summary>
    public class DerSolarUnit : DerUnit
    {
        /// <summary>
        /// DER solar unit configuration.
        /// </summary>
        private DerSolarUnitConfig _config;


        /// <summary>
        /// Power conversion system for this solar unit.
        /// </summary>
        public PowerConversionSystemBase? PowerConversionSystem { get; set; }


        /// <summary>
        /// List of one or multiple solar panels.
        /// </summary>
        public List<SolarPanelBase> SolarPanels { get; set; } = new List<SolarPanelBase>();


        /// <summary>
        /// Number of solar panels.
        /// </summary>
        public int NumberOfPanels { get => SolarPanels.Sum(s => s.NumberOfPanels); }



        /// <summary>
        /// Constructor of <see cref="DerSolarUnit"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The DER solar unit configuration.</param>
        /// <param name="derCircuit">The parent DER circuit.</param>
        public DerSolarUnit(ILogger logger, DerSolarUnitConfig config, DerCircuit derCircuit) : base(logger, config, derCircuit)
        {
            _config = config;
        }
    }
}
