
namespace paskalON.Devices.Domain.Configs.Meters.PowerMeters
{
    public class PowerMeterDeviceConfig : NameBase
    {
        /// <summary>
        /// Relationship to PowerMeterMapC37ConfigId.
        /// </summary>
        public int? PowerMeterMapC37ConfigId { get; set; }


        /// <summary>
        /// Relationship to PowerMeterMapC37Config.
        /// </summary>
        public PowerMeterMapC37Config? PowerMeterMapC37Config { get; set; }


        /// <summary>
        /// Relationship to PowerMeterMapModbusConfigId.
        /// </summary>
        public int? PowerMeterMapModbusConfigId { get; set; }


        /// <summary>
        /// Relationship to PowerMeterMapModbusConfig.
        /// </summary>
        public PowerMeterMapModbusConfig? PowerMeterMapModbusConfig { get; set; }



        /// <summary>
        /// The class name of the type to instantiate.
        /// This uniquely identifies the eventually used type of component. (e.g. a ManufacturerPcs, ManufacturerBattery etc.).
        /// </summary>
        public string? ClassName { get; set; }


        /// <summary>
        /// True if a positive value is charging. (Generation-oriented or reversed load-oriented meters.).
        /// False if a positive value means discharging. (Reversed generation-oriented or load-oriented meters.).
        /// </summary>
        public bool IsReversePowerFlow { get; set; } = false;


        /// <summary>
        /// Does this meter give both negative and positive current measurements? AC Current is often
        /// just RMS which is a non-negative magnitude but some meters may use the direction of active
        /// power flow to give a sign to the current.
        /// </summary>
        public bool IsCurrentSigned { get; set; } = false;
    }
}
