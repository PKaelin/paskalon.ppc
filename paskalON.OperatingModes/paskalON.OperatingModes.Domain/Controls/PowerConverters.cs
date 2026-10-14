// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.Controls
{
    /// <summary>
    /// Power converters for the supported power types.
    /// </summary>
    public static class PowerConverters
    {
        /// <summary>
        /// Gets the active power converter (Watts).
        /// </summary>
        public static PowerConverter<ActivePower> Active { get; } = new PowerConverter<ActivePower>(PowerAxis.Active, power => power.Watts, value => new ActivePower(value));


        /// <summary>
        /// Gets the reactive power converter (Vars).
        /// </summary>
        public static PowerConverter<ReactivePower> Reactive { get; } = new PowerConverter<ReactivePower>(PowerAxis.Reactive, power => power.VoltAmperesReactive, value => new ReactivePower(value));
    }
}
