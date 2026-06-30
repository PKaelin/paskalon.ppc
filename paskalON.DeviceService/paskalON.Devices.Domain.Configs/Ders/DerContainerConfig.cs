namespace paskalON.Devices.Domain.Configs.Ders
{
    /// <summary>
    /// DER container configuration.
    /// </summary>
    public class DerContainerConfig : DeviceIdNameBase
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
        /// Relationship id to DerUnitConfig.
        /// </summary>
        public int DerUnitConfigId { get; set; }


        /// <summary>
        /// Relationship to DerUnitConfig.
        /// </summary>
        public DerUnitConfig DerUnitConfig { get; set; } = null!;


        /// <summary>
        /// Relationship to ModbusConfig Id.
        /// </summary>
        public int? ModbusConfigId { get; set; }


        /// <summary>
        /// Relationship to ModbusConfig Id.
        /// </summary>
        public ModbusConfig? ModbusConfig { get; set; }
    }
}
