namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Type of the operating mode.
    /// </summary>
    /// <remarks>
    /// Same operating mode can be used by multiple types like MaintenanceOperatingMode, MpptOperatingMode (Maximum Power Point Tracking), etc.
    /// While others are specific to a specific type like ChargeDischargeOperatingMode
    /// </remarks>
    public enum OperatingModeType
    {
        /// <summary>
        /// Battery energy storage type: 0000000001
        /// </summary>
        Bess = 0x01,
        /// <summary>
        /// Solar energy type: 0000000010
        /// </summary>
        Solar = 0x02,
        /// <summary>
        /// Wind energy type: 0000000100
        /// </summary>
        Wind = 0x04,
        /// <summary>
        /// Nuclear energy type: 0000001000
        /// </summary>
        Nuclear = 0x08,
        /// <summary>
        /// Hydro energy type: 0000010000
        /// </summary>
        Hydro = 0x10,
        /// <summary>
        /// Gas energy type: 0000100000
        /// </summary>
        Gas = 0x20,
    }
}
