// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Maths.Randoms
{
    public static class RandomExtension
    {
        /// <summary>
        /// Returns a double number between a range (min/max)
        /// </summary>
        /// <param name="min">Minimum range</param>
        /// <param name="max">Maximum range</param>
        /// <returns>Random number in range [min, max]</returns>
        public static double NextDoubleInRange(this Random random, double min, double max)
        {
            ArgumentNullException.ThrowIfNull(random);

            if (min == double.NegativeInfinity || max == double.NegativeInfinity)
            {
                return double.MinValue;
            }

            if (min == double.PositiveInfinity || max == double.PositiveInfinity)
            {
                return double.MaxValue;
            }

            ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);

            if (min == max)
            {
                return min;
            }

            return random.NextDouble() * (max - min) + min;
        }

    }
}
