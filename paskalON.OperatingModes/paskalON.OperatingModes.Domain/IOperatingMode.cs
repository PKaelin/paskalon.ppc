// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Interface definition for all operating mode base.
    /// </summary>
    public interface IOperatingMode
    {
        /// <summary>
        /// Name of the operating mode.
        /// </summary>
        string Name { get; }


        /// <summary>
        /// Gets or sets whether operating mode is enabled in the stack or not.
        /// </summary>
        public bool IsEnabled { get; }


        /// <summary>
        /// Time stamp when operating mode was enabled the last time otherwise min value.
        /// </summary>
        public DateTimeOffset LastEnabled { get; }


        /// <summary>
        /// Gets the current operating mode state.
        /// </summary>
        OperatingModeState State { get; }


        /// <summary>
        /// Active power setpoint for the operating mode.
        /// </summary>
        /// <remarks>
        /// Setpoint is set from an external system.
        /// </remarks>
        ActivePower SetpointActivePower { get; set; }


        /// <summary>
        /// Available active power for the operating mode.
        /// </summary>
        ActivePower? AvailableActivePower { get; }


        /// <summary>
        /// Active power target for the operating mode.
        /// </summary>
        ActivePower TargetActivePower { get; }


        /// <summary>
        /// Reactive power setpoint for the operating mode.
        /// </summary>
        /// <remarks>
        /// Setpoint is set from an external system.
        /// </remarks>
        ReactivePower SetpointReactivePower { get; set; }


        /// <summary>
        /// Available reactive power for the operating mode.
        /// </summary>
        ReactivePower? AvailableReactivePower { get; }


        /// <summary>
        /// Reactive power target for the operating mode.
        /// </summary>
        ReactivePower TargetReactivePower { get; }


        /// <summary>
        /// Operating mode ramp controller for active power.
        /// </summary>
        IRampController RampControllerActive { get; }


        /// <summary>
        /// Operating mode ramp controller for reactive power.
        /// </summary>
        IRampController RampControllerReactive { get; }


        /// <summary>
        /// Operating mode curve controller.
        /// </summary>
        ICurveController? CurveController { get; }


        /// <summary>
        /// System configuration.
        /// </summary>
        SystemConfig SystemConfig { get; }


        /// <summary>
        /// Enables the operating mode.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disables the operating mode.
        /// </summary>
        void Disable();
    }

}
