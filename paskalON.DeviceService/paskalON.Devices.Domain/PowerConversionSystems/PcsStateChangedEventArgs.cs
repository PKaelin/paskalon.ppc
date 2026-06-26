namespace paskalON.Devices.Domain.PowerConversionSystems
{
    /// <summary>
    /// Event argument class for PCS state changed events.
    /// </summary>
    public class PcsStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// PCS State.
        /// </summary>
        public PcsState State { get; private set; }


        /// <summary>
        /// Constructor of <see cref="PcsStateChangedEventArgs"/>.
        /// </summary>
        /// <param name="state">PCS state.</param>
        public PcsStateChangedEventArgs(PcsState state)
        {
            State = state;
        }
    }
}
