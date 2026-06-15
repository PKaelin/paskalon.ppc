namespace paskalON.Maths.Randoms
{
    public static class RandomExtension
    {
        /// <summary>
        /// Returns a double number between a range (min/max)
        /// </summary>
        /// <param name="min">Minumum range</param>
        /// <param name="max">Maximum range</param>
        /// <returns>Random number in range [min, max]</returns>
        public static double NextDoubleInRange(this Random random, double min, double max)
        {
            if (random == null)
            {
                throw new ArgumentNullException("Random");
            }

            if (min == double.NegativeInfinity || max == double.NegativeInfinity)
            {
                return double.MinValue;
            }

            if (min == double.PositiveInfinity || max == double.PositiveInfinity)
            {
                return double.MaxValue;
            }

            if (min >= max)
            {
                return 0;
            }

            bool isNegative = random.Next(0, 1) == 0 ? false : true;

            if ((isNegative && min < 0) || max <= 0)
            {
                // NextDouble e.g. 0.0 >< 1.0 -> 0.5 * 10 + -10 = -5
                return random.NextDouble() * (0 - min) + min;
            }

            // NextDouble e.g. 0.0 >< 1.0 -> 0.5 * (10-0) + 0 = 5
            return random.NextDouble() * (max - min) + min;
        }

    }
}
