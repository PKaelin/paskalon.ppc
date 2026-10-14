// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Controls;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.Abstractions
{
    /// <summary>
    /// Interface definition for all operating modes.
    /// </summary>
    public interface IOperatingMode
    {
        /// <summary>
        /// Gets the name of the operating mode.
        /// </summary>
        string Name { get; }


        /// <summary>
        /// Gets the classifications of the operating mode resolved from the implemented classification interfaces.
        /// </summary>
        OperatingModeClassification Classification { get; }


        /// <summary>
        /// Gets whether the operating mode is activated because it is committed to the stack or the override slot.
        /// </summary>
        /// <remarks>
        /// Dont confuse this with IsActive in the configs.
        /// </remarks>
        bool IsActivated { get; }


        /// <summary>
        /// Time stamp when operating mode was activated the last time otherwise min value.
        /// </summary>
        DateTimeOffset LastActivated { get; }


        /// <summary>
        /// Gets the current operating mode state.
        /// </summary>
        /// <remarks>
        /// The operating mode state represents the current status of the operating mode, considering both active and reactive power states.
        /// </remarks>
        OperatingModeState State { get; }


        /// <summary>
        /// Gets the active power control of the operating mode.
        /// </summary>
        /// <remarks>
        /// Controls active power axis: Setpoint, TargetSetpoint, Target
        /// Null if the operating mode has no active power classification.
        /// </remarks>
        PowerControl<ActivePower>? ActiveControl { get; }


        /// <summary>
        /// Gets the reactive power control of the operating mode.
        /// </summary>
        /// <remarks>
        /// Null if the operating mode has no reactive power classification.
        /// </remarks>
        PowerControl<ReactivePower>? ReactiveControl { get; }


        /// <summary>
        /// Gets the change of the final active power caused by this operating mode in the last calculation cycle.
        /// </summary>
        ActivePower ActivePowerContribution { get; }


        /// <summary>
        /// Gets the change of the final reactive power caused by this operating mode in the last calculation cycle.
        /// </summary>
        ReactivePower ReactivePowerContribution { get; }


        /// <summary>
        /// Operating mode curve controller.
        /// </summary>
        ICurveController? CurveController { get; }


        /// <summary>
        /// Calculates the operating mode contribution and writes it into the operating mode signals.
        /// </summary>
        /// <param name="signals">The operating mode signals of the current calculation cycle.</param>
        /// <returns>The operating mode signals.</returns>
        OperatingModeSignals Calculate(OperatingModeSignals signals);
    }
}
