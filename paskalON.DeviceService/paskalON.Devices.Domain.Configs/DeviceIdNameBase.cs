namespace paskalON.Devices.Domain.Configs
{
    /// <summary>
    /// Base class for all classes that require a device ID and name.
    /// </summary>
    public abstract class DeviceIdNameBase : NameBase
    {
        /// <summary>
        /// Id of the device.
        /// </summary>
        /// <remarks>
        /// Each device subclass should have its own unique DeviceID.
        /// </remarks>
        public abstract required int DeviceId { get; set; }


        /// <summary>
        /// Whether this device is active meaning whether it should be loaded into configuration.
        /// </summary>
        public required bool IsActive { get; set; }
    }
}
