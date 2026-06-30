using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Configs.Meters.PowerMeters
{
    public class CircuitPowerMeterConfig : PowerMeterBaseConfig
    {
        /// <summary>
        /// Parent relationship to DerCircuitConfig Id.
        /// </summary>
        public int DerCircuitConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerCircuitConfig.
        /// </summary>
        public required DerCircuitConfig DerCircuitConfig { get; set; }
    }
}
