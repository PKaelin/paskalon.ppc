namespace paskalON.Devices.Domain.Configs.EnergyResources.Solars
{
    /// <summary>
    /// Solar connection type describes how the panels are connected.
    /// </summary>
    public enum SolarConnectionType
    {
        /// <summary>
        /// This configuration adds the voltage together while keeping the amperage (current) constant.
        /// </summary>
        Series = 0,
        /// <summary>
        /// This configuration adds the amperage together while keeping the voltage constant
        /// </summary>
        Parallel = 1,
    }
}
