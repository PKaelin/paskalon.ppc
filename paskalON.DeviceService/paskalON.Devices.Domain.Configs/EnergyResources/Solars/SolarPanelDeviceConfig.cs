namespace paskalON.Devices.Domain.Configs.EnergyResources.Solars
{
    /// <summary>
    /// Single solar panel configuration
    /// </summary>
    public class SolarPanelDeviceConfig : NameBase
    {
        /// <summary>
        /// The class name of the type to instantiate.
        /// This uniquely identifies the eventually used type of component. (e.g. a ManufacturerPcs, ManufacturerBattery etc.)
        /// </summary>
        public required string ClassName { get; set; }


        /// <summary>
        /// Minimum output voltage of the panel in volts.
        /// </summary>
        public double MinimumVoltage
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfNegative(value); field = value; }
        }


        /// <summary>
        /// Maximum output voltage of the panel in volts.
        /// </summary>
        public double MaximumVoltage
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfNegative(value); field = value; }
        }


        /// <summary>
        /// Minimum output current of the panel in ampere.
        /// </summary>
        public double MinimumCurrent
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfNegative(value); field = value; }
        }


        /// <summary>
        /// Maximum output current of the panel in ampere.
        /// </summary>
        public double MaximumCurrent
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfNegative(value); field = value; }
        }
    }
}
