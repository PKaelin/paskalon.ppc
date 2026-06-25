namespace paskalON.Devices.Domain.Configs.Ders
{
    /// <summary>
    /// DER container configuration.
    /// </summary>
    public class DerContainerConfig : ModbusAddressBase
    {
        /// <summary>
        /// Relationship id to DerUnitConfig.
        /// </summary>
        public int? DerUnitConfigId { get; set; }


        /// <summary>
        /// Relationship to DerUnitConfig.
        /// </summary>
        public DerUnitConfig? DerUnitConfig { get; set; }
    }
}
