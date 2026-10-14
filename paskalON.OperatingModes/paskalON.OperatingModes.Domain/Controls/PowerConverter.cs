// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Controls
{
    /// <summary>
    /// Converts a power type to its numeric base unit value and back.
    /// </summary>
    /// <typeparam name="TPower">The power type.</typeparam>
    /// <param name="Axis">The power axis of the power type.</param>
    /// <param name="ToValue">Converts the power to its base unit value (Watts or Vars).</param>
    /// <param name="FromValue">Converts the base unit value (Watts or Vars) to the power.</param>
    /// <remarks>
    /// Use <see cref="PowerConverters"/> to get the power converters for the supported power types.
    /// </remarks>
    public sealed record PowerConverter<TPower>(PowerAxis Axis, Func<TPower, double> ToValue, Func<double, TPower> FromValue) where TPower : struct
    {
        /// <summary>
        /// Gets the metric name prefix of the power axis.
        /// </summary>
        public string MetricPrefix { get => Axis == PowerAxis.Active ? "ActivePower" : "ReactivePower"; }
    }
}
