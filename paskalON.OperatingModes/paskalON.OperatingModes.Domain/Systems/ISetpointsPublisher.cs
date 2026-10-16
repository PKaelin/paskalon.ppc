// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.Systems
{
    /// <summary>
    /// Publishes the applied setpoints of the system operating mode control, which the power control distributes over the system.
    /// </summary>
    /// <remarks>
    /// Port of the domain, implemented outside the domain (e.g. by a message broker publisher).
    /// </remarks>
    public interface ISetpointsPublisher
    {
        /// <summary>
        /// Publishes the applied setpoints.
        /// </summary>
        /// <remarks>
        /// Called on the calculation cycle, so the implementation must not block (e.g. hand the message over to the broker without waiting).
        /// </remarks>
        /// <param name="setpointActivePower">The applied active power setpoint.</param>
        /// <param name="setpointReactivePower">The applied reactive power setpoint.</param>
        void Publish(ActivePower setpointActivePower, ReactivePower setpointReactivePower);


        /// <summary>
        /// Publishes the setpoints of a DER unit.
        /// </summary>
        /// <remarks>
        /// Called on the calculation cycle, so the implementation must not block (e.g. hand the message over to the broker without waiting).
        /// </remarks>
        /// <param name="derUnitName">The name of the DER unit.</param>
        /// <param name="setpointActivePower">The active power setpoint of the DER unit.</param>
        /// <param name="setpointReactivePower">The reactive power setpoint of the DER unit.</param>
        void PublishUnit(string derUnitName, ActivePower setpointActivePower, ReactivePower setpointReactivePower);
    }
}
