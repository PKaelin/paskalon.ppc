namespace paskalON.Devices.Domain.Meters.PowerMeters
{
    /// <summary>
    /// Power meter interface for the instances that communicate with the device.
    /// </summary>
    /// <typeparam name="T">The type of the power meter.</typeparam>
    public interface IPowerMeter<T> : IDevice<T>
    {
    }
}
