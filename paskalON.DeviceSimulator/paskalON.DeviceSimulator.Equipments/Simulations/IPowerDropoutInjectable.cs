// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Equipments.Simulations
{
    /// <summary>
    /// Simulated meter whose active and reactive power updates can be dropped randomly (telemetry impairment).
    /// </summary>
    public interface IPowerDropoutInjectable : ISimulatedDevice
    {
        /// <summary>
        /// Random dropout of the active power updates.
        /// </summary>
        MeasurementDropout ActivePowerDropout { get; }


        /// <summary>
        /// Random dropout of the reactive power updates.
        /// </summary>
        MeasurementDropout ReactivePowerDropout { get; }
    }
}
