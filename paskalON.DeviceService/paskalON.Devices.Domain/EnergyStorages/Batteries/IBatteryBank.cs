namespace paskalON.Devices.Domain.EnergyStorages.Batteries
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Battery bank interface for the instances that communicate with the device.
    /// </summary>
    /// <typeparam name="T">The type of the battery bank.</typeparam>
    public interface IBatteryBank<T> : IDevice<T>
    {
        /// <summary>
        /// Connects the battery bank and starts communicating once in state connected.
        /// </summary>
        void Connect();


        /// <summary>
        /// Disconnects the battery bank after it stops communicating.
        /// </summary>
        void Disconnect();
    }
}
