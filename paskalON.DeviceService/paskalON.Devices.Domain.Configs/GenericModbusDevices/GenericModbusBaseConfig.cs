namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    /// <summary>
    /// Base class for generic Modbus configurations.
    /// </summary>
    public abstract class GenericModbusBaseConfig : ModbusConfig
    {
        /// <summary>
        /// Id of the generic device.
        /// </summary>
        public required int DeviceId
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                field = value;
            }
        }


        /// <summary>
        /// Whether this device is active meaning whether it should be loaded into configuration.
        /// </summary>
        public required bool IsActive { get; set; }
    }
}
