// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Transitions;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.Systems
{
    /// <summary>
    /// Interface of the system operating mode control that applies the override modes and the operating mode stack to the supervisory setpoints.
    /// </summary>
    public interface ISystemOperatingModeControl
    {
        /// <summary>
        /// Gets the current state of the system operating mode control.
        /// </summary>
        SystemState State { get; set; }


        /// <summary>
        /// Gets the active power setpoint received from the supervisory control.
        /// </summary>
        ActivePower SetpointActivePower { get; }


        /// <summary>
        /// Gets the reactive power setpoint received from the supervisory control.
        /// </summary>
        ReactivePower SetpointReactivePower { get; }


        /// <summary>
        /// Gets the active power setpoint after applying the operating mode stack.
        /// </summary>
        /// <remarks>
        /// What it is: What is actually published to the power control service.
        /// Transitions: Included. Continuous at a change, ramps towards the objective.
        /// Plant-wide override: Replaces the value.
        /// Nameplate: Always within it.
        /// Contributions: Differs from the objective by the transition offset, plus any constraint.
        /// </remarks>
        ActivePower SetpointActivePowerActual { get; }


        /// <summary>
        /// Gets the reactive power setpoint after applying the operating mode stack.
        /// </summary>
        /// What it is: What is actually published to the power control service.
        /// Transitions: Included. Continuous at a change, ramps towards the objective.
        /// Plant-wide override: Replaces the value.
        /// Nameplate: Always within it.
        /// Contributions: Differs from the objective by the transition offset, plus any constraint.
        /// </remarks>
        ReactivePower SetpointReactivePowerActual { get; }


        /// <summary>
        /// Gets the forced setpoints of the active override modes.
        /// </summary>
        IReadOnlyList<UnitOverrideSetpoint> OverrideSetpoints { get; }


        /// <summary>
        /// Gets the active power objective calculated from the operating mode stack (applied target without transition).
        /// </summary>
        /// <remarks>
        /// What it is: What the stack asks for (signals.FinalActivePower).
        /// Transitions: Not included. jumps at a stack change.
        /// Plantwide override: Not included.
        /// Nameplate: Not applied. Can exceed it.
        /// Contributions: All modes contributions add up to this.
        /// </remarks>
        ActivePower ActivePowerObjective { get; }


        /// <summary>
        /// Gets the reactive power objective calculated from the operating mode stack (applied target without transition).
        /// </summary>
        /// <remarks>        
        /// What it is: What the stack asks for (signals.FinalReactivePower).
        /// Transitions: Not included. jumps at a stack change.
        /// Plantwide override: Not included.
        /// Nameplate: Not applied. Can exceed it.
        /// Contributions: All modes contributions add up to this.
        /// </remarks>
        ReactivePower ReactivePowerObjective { get; }


        /// <summary>
        /// Gets the transition of the active power axis.
        /// </summary>
        AxisTransition ActivePowerTransition { get; }


        /// <summary>
        /// Gets the transition of the reactive power axis.
        /// </summary>
        AxisTransition ReactivePowerTransition { get; }


        /// <summary>
        /// Updates the setpoints received from the supervisory control.
        /// </summary>
        /// <param name="activePower">The active power setpoint.</param>
        /// <param name="reactivePower">The reactive power setpoint.</param>
        void UpdateSetpoints(ActivePower activePower, ReactivePower reactivePower);


        /// <summary>
        /// Calculates one cycle of the override modes and the operating mode stack.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the calculation.</param>
        void Calculate(CancellationToken cancellationToken);
    }
}
