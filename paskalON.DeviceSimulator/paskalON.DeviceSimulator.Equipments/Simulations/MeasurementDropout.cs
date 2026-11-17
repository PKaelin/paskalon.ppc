// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Equipments.Simulations
{
    /// <summary>
    /// Telemetry impairment that randomly drops measurement updates.
    /// </summary>
    /// <remarks>
    /// A dropped update keeps the last reported value, so the receiver sees a stale measurement for that
    /// reporting interval (e.g. 1 kW, 1 kW instead of 2 kW, 3 kW). The dropout is off by default.
    /// </remarks>
    public sealed class MeasurementDropout
    {
        /// <summary>
        /// Default drop chance in percent, used until another value is set.
        /// </summary>
        public const double DefaultPercent = 5;


        /// <summary>
        /// Data lock object.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Source of random numbers.
        /// </summary>
        private readonly Random _random;


        /// <summary>
        /// Flag whether updates are dropped.
        /// </summary>
        public bool IsEnabled
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        }


        /// <summary>
        /// Chance in percent (0..100) that an update is dropped while the dropout is enabled.
        /// </summary>
        public double Percent
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = DefaultPercent;


        /// <summary>
        /// Constructor of <see cref="MeasurementDropout"/>.
        /// </summary>
        /// <param name="random">Source of random numbers.</param>
        public MeasurementDropout(Random random)
        {
            ArgumentNullException.ThrowIfNull(random);

            _random = random;
        }


        /// <summary>
        /// Switches the dropout on or off and sets the drop chance.
        /// </summary>
        /// <param name="isEnabled">True to drop updates randomly, false to report every update.</param>
        /// <param name="percent">Chance in percent (0..100) that an update is dropped.</param>
        /// <exception cref="ArgumentOutOfRangeException">The percent is not within 0..100.</exception>
        public void Set(bool isEnabled, double percent)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(percent, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(percent, 100);

            lock (_dataLock)
            {
                IsEnabled = isEnabled;
                Percent = percent;
            }
        }


        /// <summary>
        /// Decides whether the next update is dropped.
        /// </summary>
        /// <returns>True when the update is dropped and the last value shall be kept.</returns>
        public bool IsDropped()
        {
            lock (_dataLock)
            {
                if (IsEnabled is false || Percent <= 0)
                {
                    return false;
                }

                return _random.NextDouble() * 100 < Percent;
            }
        }
    }
}
