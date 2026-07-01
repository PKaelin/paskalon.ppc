namespace paskalON.Devices.Domain.EnergyStorages.Batteries
{
    /// <summary>
    /// Event argument class for battery bank state changed events.
    /// </summary>
    public class BatteryBankStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Battery bank state.
        /// </summary>
        public BatteryBankState State { get; private set; }


        /// <summary>
        /// Constructor of <see cref="BatteryBankStateChangedEventArgs"/>.
        /// </summary>
        /// <param name="state">The battery bank state.</param>
        public BatteryBankStateChangedEventArgs(BatteryBankState state)
        {
            State = state;
        }
    }
}
