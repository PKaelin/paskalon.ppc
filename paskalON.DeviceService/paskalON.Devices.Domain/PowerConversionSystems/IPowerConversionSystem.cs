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


        /// <summary>
        /// Puts the power conversion system in standby mode.
        /// </summary>
        /// <remarks>
        /// The standby mode shall have a minimum active power target configured in the PCS.
        /// This could be required for PCSs that need a minimum active power to be able to switch on properly.
        /// If not standby active power is provided, the PCS will use the minimum active power target configured in the PCS.
        /// </remarks>
        void Standby(double? standbyActivePower = null);
    }
}
