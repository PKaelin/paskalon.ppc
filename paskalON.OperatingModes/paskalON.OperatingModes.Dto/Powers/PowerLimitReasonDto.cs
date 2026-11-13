// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Dto.Powers
{
    /// <summary>
    /// Reason why the applied setpoint of a power axis differs from its supervisory setpoint
    /// (values of <see cref="CurrentSetpointsDto.ActivePowerLimitReason"/> and <see cref="CurrentSetpointsDto.ReactivePowerLimitReason"/>).
    /// </summary>
    /// <remarks>
    /// The values are ordered by priority: if several reasons apply, the highest value is reported.
    /// </remarks>
    public enum PowerLimitReasonDto
    {
        /// <summary>
        /// The applied setpoint is the supervisory setpoint.
        /// </summary>
        None = 0,


        /// <summary>
        /// The operating modes calculate another power by design, e.g. closed loop feedback, droop or maximum power point tracking.
        /// </summary>
        OperatingMode = 1,


        /// <summary>
        /// The applied setpoint ramps to the setpoint.
        /// </summary>
        Ramping = 2,


        /// <summary>
        /// The setpoint is limited by the minimum/maximum limits of the operating mode or the nameplate of the system.
        /// </summary>
        ConfigurationLimit = 3,


        /// <summary>
        /// The setpoint is limited by the available power, e.g. an empty or full battery or low solar production,
        /// or the operating mode waits for the available power.
        /// </summary>
        AvailablePower = 4,


        /// <summary>
        /// A limiting operating mode limits the power.
        /// </summary>
        LimitingMode = 5,


        /// <summary>
        /// No operating mode controls the power axis.
        /// </summary>
        NoOperatingMode = 6,


        /// <summary>
        /// A plant-wide override operating mode forces the power, e.g. a shutdown.
        /// </summary>
        Override = 7
    }
}
