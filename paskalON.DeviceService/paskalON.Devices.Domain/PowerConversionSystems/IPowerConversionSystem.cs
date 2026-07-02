namespace paskalON.Devices.Domain.PowerConversionSystems
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Power conversion system interface for the instances that communicate with the device.
    /// </summary>
    /// <typeparam name="T">The type of the PCS.</typeparam>
    public interface IPowerConversionSystem<T> : IDevice<T>
    {
        /// <summary>
        /// Starts the power conversion system.
        /// </summary>
        void Start();


        /// <summary>
        /// Stops the power conversion system.
        /// </summary>
        void Stop();
    }
}
