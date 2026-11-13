// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Controls;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.Systems
{
    /// <summary>
    /// Explains the applied setpoints: the supervisory setpoints they are calculated from and why they differ.
    /// </summary>
    /// <param name="SupervisoryActivePower">
    /// The system wide supervisory active power setpoint (e.g. of the operator).
    /// </param>
    /// <param name="SupervisoryReactivePower">
    /// The system wide supervisory reactive power setpoint (e.g. of the operator).
    /// </param>
    /// <param name="ActivePowerLimitReason">
    /// Why the applied active power differs from the supervisory active power setpoint.
    /// </param>
    /// <param name="ReactivePowerLimitReason">
    /// Why the applied reactive power differs from the supervisory reactive power setpoint.
    /// </param>
    public sealed record SetpointsStatus(ActivePower SupervisoryActivePower, ReactivePower SupervisoryReactivePower,
        PowerLimitReason ActivePowerLimitReason, PowerLimitReason ReactivePowerLimitReason);
}
