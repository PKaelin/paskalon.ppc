// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Power range with the sign convention of the setpoints.
    /// </summary>
    /// <remarks>
    /// Active power: the minimum (&lt;= 0) is the charge/consumption limit, the maximum (&gt;= 0) the discharge/production limit.
    /// </remarks>
    /// <typeparam name="TPower">The power type, e.g. <see cref="ActivePower"/>.</typeparam>
    /// <param name="Minimum">
    /// The minimum power.
    /// </param>
    /// <param name="Maximum">
    /// The maximum power.
    /// </param>
    public readonly record struct PowerRange<TPower>(TPower Minimum, TPower Maximum) where TPower : struct;


    /// <summary>
    /// Factory methods of <see cref="PowerRange{TPower}"/>.
    /// </summary>
    public static class PowerRange
    {
        /// <summary>
        /// Creates a symmetric active power range from -|available| to +|available|.
        /// </summary>
        /// <param name="available">
        /// The available active power in both directions.
        /// </param>
        /// <returns>
        /// The symmetric active power range.
        /// </returns>
        public static PowerRange<ActivePower> Symmetric(ActivePower available)
        {
            double watts = Math.Abs(available.Watts);

            return new PowerRange<ActivePower>(new ActivePower(0 - watts), new ActivePower(watts));
        }
    }
}
