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
        public string? ClassName { get; set; }


        /// <summary>
        /// Gets or sets the minimum output voltage of the panel in volts.
        /// </summary>
        public double MinimumVoltage
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfNegative(value); field = value; }
        }


        /// <summary>
        /// Gets or sets the maximum output voltage of the panel in volts.
        /// </summary>
        public double MaximumVoltage
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfNegative(value); field = value; }
        }


        /// <summary>
        /// Gets or sets the minimum output Current of the panel in ampere.
        /// </summary>
        public double MinimumCurrent
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfNegative(value); field = value; }
        }


        /// <summary>
        /// Gets or sets the maximum output Current of the panel in ampere.
        /// </summary>
        public double MaximumCurrent
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfNegative(value); field = value; }
        }


        /// <summary>
        /// Gets or sets the maximum output Output of the panel in watts.
        /// </summary>
        public double MaximumOutput
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfNegative(value); field = value; }
        }
    }
}
