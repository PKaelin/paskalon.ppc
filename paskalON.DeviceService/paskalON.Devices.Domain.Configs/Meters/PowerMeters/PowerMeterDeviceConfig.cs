
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
        /// Indicates whether it is a reverse power flow.
        /// </summary>
        /// <remarks>
        /// True if a positive value is charging (Generation-oriented or reversed load-oriented meters).
        /// False if a positive value means discharging (Reversed generation-oriented or load-oriented meters).
        /// </remarks>
        public bool IsReversePowerFlow { get; set; }


        /// <summary>
        /// Indicates whether current can handle positive and negative values.
        /// If it cannot handle negative values the current has to be handled according to the power flow.
        /// </summary>
        /// <remarks>
        /// True if current is signed and can do negative values.
        /// False if current is not signed.
        /// </remarks>
        public bool IsCurrentSigned { get; set; }



    }
}
