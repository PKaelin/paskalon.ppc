using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Configs.EnergyResources.Solars
{
    /// <summary>
    /// Minimum configuration for solar panel.
    /// At the moment we assume we dont control solar panels. Hence only one solar panel config is references.
    /// </summary>
    public class SolarPanelConfig : DeviceIdNameBase
    {
        /// <summary>
        /// Id of the device.
        /// </summary>
        public override required int DeviceId
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                field = value;
            }
        }

        /// <summary>
        /// Parent relationship to DerUnitConfig Id.
        /// </summary>
        public int DerUnitConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerUnitConfig.
        /// </summary>
        public required DerUnitConfig DerUnitConfig { get; set; }


        /// <summary>
        /// Relationship to SolarPanelDeviceConfig Id.
        /// </summary>
        public int SolarPanelDeviceConfigId { get; set; }


        /// <summary>
        /// Relationship to SolarPanelDeviceConfig.
        /// </summary>
        public required SolarPanelDeviceConfig SolarPanelDeviceConfig { get; set; }


        /// <summary>
        /// Number of solar panels.
        /// </summary>
        public int NumberOfPanels { get; set; } = 0;


        /// <summary>
        /// Solar connection type describes how the panels are connected.
        /// </summary>
        public SolarConnectionType ConnectionType { get; set; }


        /// <summary>
        /// Simulate list of solar panels by multiplying the solar device configured MinimumVoltageSum.
        /// </summary>
        public double MinimumVoltageSum
        {
            get => SolarPanelDeviceConfig == null ? 0 : ConnectionType == SolarConnectionType.Series ? SolarPanelDeviceConfig.MinimumVoltage * NumberOfPanels : SolarPanelDeviceConfig.MinimumVoltage;
        }


        /// <summary>
        /// Simulate list of solar panels by multiplying the solar device configured MaximumVoltageSum.
        /// </summary>
        public double MaximumVoltageSum
        {
            get => SolarPanelDeviceConfig == null ? 0 : ConnectionType == SolarConnectionType.Series ? SolarPanelDeviceConfig.MaximumVoltage * NumberOfPanels : SolarPanelDeviceConfig.MaximumVoltage;
        }


        /// <summary>
        /// Simulate list of solar panels by multiplying the solar device configured MaximumOutputSum.
        /// </summary>
        public double MaximumOutputSum
        {
            get => SolarPanelDeviceConfig == null ? 0 : ConnectionType == SolarConnectionType.Series ? SolarPanelDeviceConfig.MaximumOutput * NumberOfPanels : SolarPanelDeviceConfig.MaximumOutput;
        }

    }
}

